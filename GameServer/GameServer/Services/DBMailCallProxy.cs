using GameServer.Common;
using Google.Protobuf;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace GameServer.Services
{
    public class DBMailCallProxy
    {
        public event Func<DBMailQueueType, Task>? EventCancelled;
        private CancellationTokenSource? Sourse { get; set; }
        private readonly Mailer.MailerClient _client;
        private readonly DBMailQueueRepository _dbMailQueueRepository;
        private readonly DBMailQueueType _type;
        private readonly ILogger _logger;

        public DBMailCallProxy(Mailer.MailerClient client, 
            DBMailQueueRepository dbMailQueueRepository, 
            DBMailQueueType type,
            ILogger logger)
        {
            _client = client;
            _dbMailQueueRepository = dbMailQueueRepository;
            _type = type;
            _logger = logger;

            EventCancelled += OnCancelled;
        }

        private async Task OnCancelled(DBMailQueueType obj)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            Start();
        }

        public void Start()
        {
            Sourse = new CancellationTokenSource();

            var callOptions = new CallOptions(new Metadata { new Metadata.Entry("mailbox-name", "game") }, cancellationToken : Sourse.Token);
            var _call = _client.Mailbox(callOptions.WithWaitForReady());

            var outgoMailQueue = _dbMailQueueRepository.GetOrAddOutgoMailQueue(_type);
            outgoMailQueue.OnRead += WriteDBMail;

            _ = Task.Run(async () =>
            {
                var incomeMailQueue = _dbMailQueueRepository.GetIncomeMailQueue();
                await foreach (var message in _call.ResponseStream.ReadAllAsync(Sourse.Token))
                {
                    await incomeMailQueue.WriteAsync(new MailPacket { Id = message.Id, Content = message.Content.ToByteArray(), Reserve = message.Reserve });
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("!!!end");
                Console.ResetColor();
            }, Sourse.Token);


            async Task WriteDBMail(MailPacket mail)
            {
                var forward = new ForwardMailMessage
                {
                    Id = mail.Id,
                    Content = mail.Content != null ? Google.Protobuf.ByteString.CopyFrom(mail.Content) : Google.Protobuf.ByteString.Empty,
                    Reserve = mail.ClientId
                };
                try
                {
                    await _call.RequestStream.WriteAsync(forward);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);
                    outgoMailQueue.OnRead -= WriteDBMail;
                    Sourse.Cancel();

                    EventCancelled?.Invoke(_type);
                }
            }
        }
    }
}
