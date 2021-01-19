using GameServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace GameServer.Services
{
    public class OutgoMailQueue<TKey>
    {
        private readonly Channel<MailPacket> _mailChannel;
        private int _totalMailCount;

        public TKey Key { get; }
        public event Func<MailPacket, Task>? OnRead;
        public event Action? OnComplete;

        public OutgoMailQueue(TKey key)
        {
            Key = key;
            _mailChannel = Channel.CreateUnbounded<MailPacket>();
            _totalMailCount = 0;

            _ = Task.Run(async () =>
              {
                  await foreach (var mail in _mailChannel.Reader.ReadAllAsync())
                  {
                      Interlocked.Decrement(ref _totalMailCount);

                      if (OnRead != null)
                      {
                          await OnRead.Invoke(mail);
                      }
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

        public bool TryWriteMail(MailPacket mail)
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
