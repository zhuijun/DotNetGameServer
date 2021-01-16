using System.Linq;
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
        private readonly MailQueueRepository _mailQueueRepository;

        public MailerService(ILoggerFactory loggerFactory, MailQueueRepository mailQueueRepository)
        {
            _logger = loggerFactory.CreateLogger<MailerService>();
            _mailQueueRepository = mailQueueRepository;
        }

        public async override Task Mailbox(
            IAsyncStreamReader<ForwardMailMessage> requestStream,
            IServerStreamWriter<MailboxMessage> responseStream,
            ServerCallContext context)
        {
            long clientId = _mailQueueRepository.CreateClientId();
            var outgoMailQueue = _mailQueueRepository.GetOutgoMailQueue(clientId);
            outgoMailQueue.OnRead += DoWrite;
            
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            outgoMailQueue.OnComplete += DoCompte;

            var incomeMailQueue = _mailQueueRepository.GetIncomeMailQueue();

            try
            {
                while (await requestStream.MoveNext(token))
                {
                    var request = requestStream.Current;

                    var mail = new MailMessage(clientId, request.Id, request.Content.ToByteArray());
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
                _mailQueueRepository.RemoveOutgoMailQueue(clientId);
            }

            async Task DoWrite(MailMessage mail)
            {
                await responseStream.WriteAsync(new MailboxMessage
                {
                    Id = mail.Id,
                    Content = Google.Protobuf.ByteString.CopyFrom(mail.Content),
                });
            }

            async void DoCompte()
            {
                await DoWrite(new MailMessage(clientId, 999999, null));
                source.Cancel();
            }
        }
    }
}
