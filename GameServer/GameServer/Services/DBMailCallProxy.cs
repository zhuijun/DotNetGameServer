﻿using GameServer.Common;
using Google.Protobuf;
using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
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
        private readonly Channel<MailPacket> _mailChannel;

        private MailPacket? CurrentMail { get; set; }

        public DBMailCallProxy(Mailer.MailerClient client, 
            DBMailQueueRepository dbMailQueueRepository, 
            DBMailQueueType type,
            ILogger logger)
        {
            _client = client;
            _dbMailQueueRepository = dbMailQueueRepository;
            _type = type;
            _logger = logger;
            _mailChannel = Channel.CreateUnbounded<MailPacket>();
            var outgoMailQueue = _dbMailQueueRepository.GetOrAddOutgoMailQueue(_type);
            outgoMailQueue.OnRead += OnRead;
            EventCancelled += OnCancelled;
        }

        private async Task OnCancelled(DBMailQueueType obj)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            Start();
        }

        private async Task OnRead(MailPacket mail)
        {
            await _mailChannel.Writer.WriteAsync(mail);
        }

        public void Start()
        {
            if (Sourse != null && !Sourse.IsCancellationRequested)
            {
                return;
            }

            Sourse = new CancellationTokenSource();

            var callOptions = new CallOptions(new Metadata { new Metadata.Entry("mailbox-name", "game") }, cancellationToken : Sourse.Token);
            var _call = _client.Mailbox(callOptions.WithWaitForReady());

            _ = Task.Run(async () =>
            {
                var incomeMailQueue = _dbMailQueueRepository.GetIncomeMailQueue();
                await foreach (var message in _call.ResponseStream.ReadAllAsync(Sourse.Token))
                {
                    await incomeMailQueue.WriteAsync(new MailPacket { Id = message.Id, Content = message.Content.ToByteArray(), 
                        Reserve = message.Reserve, UserId = message.UserId, ClientId = message.ClientId });
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("!!!end");
                Console.ResetColor();
            }, Sourse.Token);


            _ = Task.Run(async () =>
            {
                while (!Sourse.Token.IsCancellationRequested)
                {
                    try
                    {
                        if (CurrentMail == null)
                        {
                            if (await _mailChannel.Reader.WaitToReadAsync(Sourse.Token))
                            {
                                if (_mailChannel.Reader.TryRead(out var mail))
                                {
                                    CurrentMail = mail;
                                    await WriteDBMail(CurrentMail);
                                    CurrentMail = null;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            await WriteDBMail(CurrentMail);
                            CurrentMail = null;
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogWarning(e.Message);
                        Sourse.Cancel();
                    }
                }
            }, Sourse.Token);

            async Task WriteDBMail(MailPacket mail)
            {
                var forward = new ForwardMailMessage
                {
                    Id = mail.Id,
                    Content = mail.Content != null ? Google.Protobuf.ByteString.CopyFrom(mail.Content) : Google.Protobuf.ByteString.Empty,
                    Reserve = mail.ClientId, UserId = mail.UserId, ClientId = mail.ClientId
                };
                try
                {
                    await _call.RequestStream.WriteAsync(forward);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);
                    Sourse.Cancel();
                    EventCancelled?.Invoke(_type);
                }
            }
        }
    }
}
