using Google.Protobuf;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;
using ServicesCore.Common;
using ServicesCore.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class MailerService : Mailer.MailerBase
    {
        private readonly ILogger _logger;
        private readonly AgentMailQueueRepository _agentMailQueueRepository;
        private readonly AgentClientIdProvider _agentClientIdProvider;
        public Dispatcher Dispatcher { get; }

        public MailerService(ILoggerFactory loggerFactory, 
            AgentMailQueueRepository agentMailQueueRepository,
            AgentClientIdProvider agentClientIdProvider,
            Dispatcher dispatcher)
        {
            _logger = loggerFactory.CreateLogger<MailerService>();
            _agentMailQueueRepository = agentMailQueueRepository;
            _agentClientIdProvider = agentClientIdProvider;
            Dispatcher = dispatcher;
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
                        var metadata = context.RequestHeaders.SingleOrDefault(e => e.Key == "metadata" + Metadata.BinaryHeaderSuffix).ValueBytes;
                        var mailMetadata = Mail.MailMetadata.Parser.ParseFrom(metadata);
                        long userId = mailMetadata.UserId;
                        var nickname = mailMetadata.NickName;
                        var headicon = mailMetadata.HeadIcon;

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
                                //_logger.LogInformation($"request mail: {request.Id}");
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
                                Id = (int)AgentGameProto.MessageId.EnterGameRequestId,
                                Content = new AgentGameProto.EnterGameRequest
                                {
                                    UserId = userId,
                                    NickName = nickname,
                                    HeadIcon = headicon
                                }.ToByteArray(),
                                ClientId = clientId,
                                UserId = userId
                            };
                            Dispatcher.WriteInnerMail(mail);
                            await Task.CompletedTask;
                        }

                        async Task LeaveGame()
                        {
                            //退出游戏
                            var mail1 = new MailPacket
                            {
                                Id = (int)AgentGameProto.MessageId.BeforeLeaveGameRequestId,
                                Content = new AgentGameProto.BeforeLeaveGameRequest
                                {
                                    UserId = userId
                                }.ToByteArray(),
                                ClientId = clientId,
                                UserId = userId
                            };
                            Dispatcher.WriteInnerMail(mail1);
                            await Task.CompletedTask;

                            var mail2 = new MailPacket
                            {
                                Id = (int)AgentGameProto.MessageId.LeaveGameRequestId,
                                Content = new AgentGameProto.LeaveGameRequest
                                {
                                    UserId = userId
                                }.ToByteArray(),
                                ClientId = clientId,
                                UserId = userId
                            };
                            Dispatcher.WriteInnerMail(mail2);
                            await Task.CompletedTask;
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
