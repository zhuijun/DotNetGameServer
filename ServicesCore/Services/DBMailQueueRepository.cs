using ServicesCore.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesCore.Services
{
    public class DBMailQueueRepository
    {
        private readonly ConcurrentDictionary<DBMailQueueType, OutgoMailQueue<DBMailQueueType>> _outgoMailQueues = new ConcurrentDictionary<DBMailQueueType, OutgoMailQueue<DBMailQueueType>>();

        private readonly IncomeMailQueue _incomeMailQueue = new IncomeMailQueue();

        public IncomeMailQueue GetIncomeMailQueue()
        {
            return _incomeMailQueue;
        }

        public OutgoMailQueue<DBMailQueueType> GetOrAddOutgoMailQueue(DBMailQueueType type)
        {
            return _outgoMailQueues.GetOrAdd(type, (n) => new OutgoMailQueue<DBMailQueueType>(n));
        }

        public bool TryRemoveOutgoMailQueue(DBMailQueueType type)
        {
            var r = _outgoMailQueues.TryRemove(type, out var _);
            return r;
        }
    }
}
