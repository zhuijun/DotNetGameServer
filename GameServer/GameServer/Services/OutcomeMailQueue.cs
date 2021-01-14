using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class OutcomeMailQueue
    {
        private readonly Channel<Mail> _mailChannel;
        private int _totalMailCount;

        public long Key { get; }

        public OutcomeMailQueue(long key)
        {
            Key = key;
            _mailChannel = Channel.CreateUnbounded<Mail>();
            _totalMailCount = 0;
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
