﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Common;
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
                        long userId = long.Parse(userIdentifier);
                        long clientId = _agentClientIdProvider.CreateClientId();
                        _agentClientIdProvider.SetUserClientId(userId, clientId);

                        var outgoMailQueue = _agentMailQueueRepository.GetOutgoMailQueue(clientId);
                        outgoMailQueue.OnRead += DoWrite;

                        CancellationTokenSource source = new CancellationTokenSource();
                        CancellationToken token = source.Token;
                        outgoMailQueue.OnComplete += DoCompte;

                        var incomeMailQueue = _agentMailQueueRepository.GetIncomeMailQueue();

                        try
                        {
                            while (await requestStream.MoveNext(token))
                            {
                                var request = requestStream.Current;

                                var mail = new MailPacket { Id = request.Id, Content = request.Content.ToByteArray(), Reserve = request.Reserve, ClientId = clientId, UserId = userId };
                                await incomeMailQueue.WriteAsync(mail);
                                _logger.LogInformation($"request mail: {request.Id}");
                            }
                        }
                        finally
                        {
                            if (!source.IsCancellationRequested)
                            {
                                outgoMailQueue.Complete();
                            }

                            outgoMailQueue.OnRead -= DoWrite;
                            outgoMailQueue.OnComplete -= DoCompte;
                            _agentMailQueueRepository.RemoveOutgoMailQueue(clientId);
                        }

                        async Task DoWrite(MailPacket mail)
                        {
                            await responseStream.WriteAsync(new MailboxMessage
                            {
                                Id = mail.Id,
                                Content = Google.Protobuf.ByteString.CopyFrom(mail.Content),
                            });
                        }

                        async void DoCompte()
                        {
                            source.Cancel();
                            await DoWrite(new MailPacket { Id = 999999, ClientId = clientId });
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
