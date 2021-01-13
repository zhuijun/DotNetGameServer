﻿using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;

namespace GameServer.Services
{
    public class MailerService : Mailer.MailerBase
    {
        private readonly ILogger _logger;
        private readonly MailQueueRepository _messageQueueRepository;

        public MailerService(ILoggerFactory loggerFactory, MailQueueRepository messageQueueRepository)
        {
            _logger = loggerFactory.CreateLogger<MailerService>();
            _messageQueueRepository = messageQueueRepository;
        }

        public async override Task Mailbox(
            IAsyncStreamReader<ForwardMailMessage> requestStream,
            IServerStreamWriter<MailboxMessage> responseStream,
            ServerCallContext context)
        {
            var mailboxName = context.RequestHeaders.Single(e => e.Key == "mailbox-name").Value;

            var mailQueue = _messageQueueRepository.GetMailQueue(mailboxName);

            _logger.LogInformation($"Connected to {mailboxName}");

            mailQueue.Changed += ReportChanges;

            try
            {
                while (await requestStream.MoveNext())
                {
                    if (mailQueue.TryForwardMail(out var message))
                    {
                        _logger.LogInformation($"Forwarded mail: {message.Content}");
                    }
                    else
                    {
                        _logger.LogWarning("No mail to forward.");
                    }
                }
            }
            finally
            {
                mailQueue.Changed -= ReportChanges;
            }

            _logger.LogInformation($"{mailboxName} disconnected");

            async Task ReportChanges((int totalCount, int fowardCount, MailboxMessage.Types.Reason reason) state)
            {
                await responseStream.WriteAsync(new MailboxMessage
                {
                    Forwarded = state.fowardCount,
                    New = state.totalCount - state.fowardCount,
                    Reason = state.reason
                });
            }
        }
    }
}
