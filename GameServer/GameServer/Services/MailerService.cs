using System.Linq;
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
            long clientId = _messageQueueRepository.CreateClientId();
            var outgoMailQueue = _messageQueueRepository.GetOutgoMailQueue(clientId);
            outgoMailQueue.OnRead += DoWrite;

            var incomeMailQueue = _messageQueueRepository.GetIncomeMailQueue();

            try
            {
                while (await requestStream.MoveNext())
                {
                    var request = requestStream.Current;

                    var mail = new Mail(request.Id, request.Content.ToByteArray());
                    await incomeMailQueue.WriteAsync(mail);
                    _logger.LogInformation($"request mail: {request.Id}");

                    //test
                    if (incomeMailQueue.TryReadMail(out var mail1))
                    {
                        outgoMailQueue.TryWriteMail(mail1);
                    }
                }
            }
            finally
            {
                outgoMailQueue.OnRead -= DoWrite;
                _messageQueueRepository.RemoveOutgoMailQueue(clientId);
            }

            async Task DoWrite(Mail mail)
            {
                await responseStream.WriteAsync(new MailboxMessage
                {
                    Id = mail.Id,
                    Content = Google.Protobuf.ByteString.CopyFrom(mail.Content),
                });
            }
        }
    }
}
