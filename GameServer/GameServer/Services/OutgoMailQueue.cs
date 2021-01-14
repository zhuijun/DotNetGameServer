using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace GameServer.Services
{
    public class OutgoMailQueue
    {
        private readonly Channel<Mail> _mailChannel;
        private int _totalMailCount;

        public long Key { get; }
        public event Func<Mail, Task>? OnRead;

        public OutgoMailQueue(long key)
        {
            Key = key;
            _mailChannel = Channel.CreateUnbounded<Mail>();
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
        }

        public bool TryWriteMail(Mail mail)
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
