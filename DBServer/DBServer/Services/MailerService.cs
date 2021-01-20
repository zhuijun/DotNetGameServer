using DBServer.Interfaces;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBServer.Services
{
    public class MailerService : Mailer.MailerBase
    {
        private readonly ILogger<MailerService> _logger;
        private readonly IMessageHandle _messageHandle;

        public MailerService(ILoggerFactory loggerFactory, IMessageHandle messageHandle)
        {
            _logger = loggerFactory.CreateLogger<MailerService>();
            _messageHandle = messageHandle;
        }

        public async override Task Mailbox(
            IAsyncStreamReader<ForwardMailMessage> requestStream,
            IServerStreamWriter<MailboxMessage> responseStream,
            ServerCallContext context)
        {

            async Task replyMailAction(MailboxMessage replayMail)
            {
                await responseStream.WriteAsync(replayMail);
            }

            try
            {
                await foreach (var request in requestStream.ReadAllAsync())
                {
                    await _messageHandle.HandleMessage(request, replyMailAction);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
            }
            finally
            {
                _logger.LogInformation("Mailbox finally");
            }
        }
    }
}
