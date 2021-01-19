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

        public MailerService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MailerService>();
        }

        public async override Task Mailbox(
            IAsyncStreamReader<ForwardMailMessage> requestStream,
            IServerStreamWriter<MailboxMessage> responseStream,
            ServerCallContext context)
        {
            try
            {
                await foreach (var request in requestStream.ReadAllAsync())
                {
                    await responseStream.WriteAsync(new MailboxMessage
                    {
                        Id = request.Id,
                        Content = request.Content,
                    });
                    _logger.LogInformation($"request mail: {request.Id}");
                }
            }
            finally
            {
                _logger.LogInformation("Mailbox finally");
            }
        }
    }
}
