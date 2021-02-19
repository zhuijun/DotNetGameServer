using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Common;
using Google.Protobuf;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;

namespace GameServer.Services
{
    public class MailerService : Mailer.MailerBase
    {
        private readonly ILogger _logger;
        private readonly AgentMailQueueRepository _agentMailQueueRepository;
        private readonly AgentClientIdProvider _agentClientIdProvider;

        public MailerService(ILoggerFactory loggerFactory, 
            AgentMailQueueRepository agentMailQueueRepository,
            AgentClientIdProvider agentClientIdProvider)
        {
            _logger = loggerFactory.CreateLogger<MailerService>();
            _agentMailQueueRepository = agentMailQueueRepository;
            _agentClientIdProvider = agentClientIdProvider;
        }

        public async override Task Mailbox(
            IAsyncStreamReader<ForwardMailMessage> requestStream,
            IServerStreamWriter<MailboxMessage> responseStream,
            ServerCallContext context)
        {
            var mailboxName = context.RequestHeaders.Single(e => e.Key == "mailbox-name").Value;

            switch (mailboxName)
            {
                case "agent":
                    {
                        var userIdentifier = context.RequestHeaders.SingleOrDefault(e => e.Key == "user-identifier").Value;
                        var nickname = context.RequestHeaders.SingleOrDefault(e => e.Key == "nickname").Value;
                        long userId = long.Parse(userIdentifier);
                        long oldClientId = _agentClientIdProvider.GetUserClientId(userId);
                        if (oldClientId > 0)
                        {
                            var oldOutgoMailQueue = _agentMailQueueRepository.TryGetOutgoMailQueue(oldClientId);
                            if (oldOutgoMailQueue != null)
                            {
                                await oldOutgoMailQueue.Complete();
                            }
                        }

                        var incomeMailQueue = _agentMailQueueRepository.GetIncomeMailQueue();

                        long clientId = _agentClientIdProvider.CreateClientId();
                        _agentClientIdProvider.SetUserClientId(userId, clientId);

                        var outgoMailQueue = _agentMailQueueRepository.GetOrAddOutgoMailQueue(clientId);
                        outgoMailQueue.OnRead += DoWrite;

                        CancellationTokenSource source = new CancellationTokenSource();
                        outgoMailQueue.OnComplete += DoCompte;

                        await JoinGame();

                        try
                        {
                            await foreach (var request in requestStream.ReadAllAsync())
                            {
                                var mail = new MailPacket { Id = request.Id, Content = request.Content.ToByteArray(), Reserve = request.Reserve, ClientId = clientId, UserId = userId };
                                await incomeMailQueue.WriteAsync(mail);
                                _logger.LogInformation($"request mail: {request.Id}");
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning(e.Message);
                        }
                        finally
                        {
                            if (!source.IsCancellationRequested)
                            {
                                source.Cancel();
                                await outgoMailQueue.Complete();
                                await LeaveGame();
                                _agentClientIdProvider.RemoveUserClientId(userId);
                            }

                            outgoMailQueue.OnRead -= DoWrite;
                            outgoMailQueue.OnComplete -= DoCompte;
                            _agentMailQueueRepository.TryRemoveOutgoMailQueue(clientId);
                        }

                        async Task DoWrite(MailPacket mail)
                        {
                            await responseStream.WriteAsync(new MailboxMessage
                            {
                                Id = mail.Id,
                                Content = mail.Content != null ? Google.Protobuf.ByteString.CopyFrom(mail.Content) : Google.Protobuf.ByteString.Empty,
                            });
                        }

                        async Task DoCompte()
                        {
                            if (!source.IsCancellationRequested)
                            {
                                source.Cancel();
                                await DoWrite(new MailPacket { Id = 999999, ClientId = clientId });
                                await LeaveGame();
                            }
                        }

                        async Task JoinGame()
                        {
                            //进入游戏
                            var mail = new MailPacket
                            {
                                Id = (int)AgentGameProto.MessageID.AtoGjoinGameRequestId,
                                Content = new AgentGameProto.AtoGJoinGameRequest
                                {
                                    UserID = userId,
                                    NickName = nickname
                                }.ToByteArray(),
                                ClientId = clientId,
                                UserId = userId
                            };
                            await incomeMailQueue.WriteAsync(mail);
                        }

                        async Task LeaveGame()
                        {
                            //退出游戏
                            var mail = new MailPacket
                            {
                                Id = (int)AgentGameProto.MessageID.AtoGleaveGameRequestId,
                                Content = new AgentGameProto.AtoGLeaveGameRequest
                                {
                                    UserID = userId
                                }.ToByteArray(),
                                ClientId = clientId,
                                UserId = userId
                            };
                            await incomeMailQueue.WriteAsync(mail);
                        }
                    }

                    break;
                default:
                    {
                        //CancellationTokenSource source = new CancellationTokenSource();
                    }
                    break;
            }
        }
    }
}
