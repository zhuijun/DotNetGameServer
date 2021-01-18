﻿using GameServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace GameServer.Services
{
    public class OutgoMailQueue<K>
    {
        private readonly Channel<MailMessage> _mailChannel;
        private int _totalMailCount;

        public K Key { get; }
        public event Func<MailMessage, Task>? OnRead;
        public event Action? OnComplete;

        public OutgoMailQueue(K key)
        {
            Key = key;
            _mailChannel = Channel.CreateUnbounded<MailMessage>();
            _totalMailCount = 0;

            Task.Run(async () =>
            {
                await foreach(var mail in _mailChannel.Reader.ReadAllAsync())
                {
                    Interlocked.Decrement(ref _totalMailCount);
                    OnRead?.Invoke(mail);
                }

                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("OutgoMailQueue !!!end");
                //Console.ResetColor();
            });
        }

        public void  Complete()
        {
            _mailChannel.Writer.Complete();
            OnComplete?.Invoke();
        }

        public bool TryWriteMail(MailMessage mail)
        {
            if (_mailChannel.Writer.TryWrite(mail))
            {
                Interlocked.Increment(ref _totalMailCount);
                return true;
            }
            return false;
        }
    }
}
