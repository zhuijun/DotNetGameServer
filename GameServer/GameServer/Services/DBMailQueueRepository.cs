using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public enum DBMailQueueType
    {
        Role = 1,
        Other
    }

    public class DBMailQueueRepository
    {
        private readonly ConcurrentDictionary<DBMailQueueType, OutgoMailQueue<DBMailQueueType>> _outgoMailQueues = new ConcurrentDictionary<DBMailQueueType, OutgoMailQueue<DBMailQueueType>>();

        private readonly IncomeMailQueue _incomeMailQueue = new IncomeMailQueue();

        public IncomeMailQueue GetIncomeMailQueue()
        {
            return _incomeMailQueue;
        }

        public OutgoMailQueue<DBMailQueueType> GetOutgoMailQueue(DBMailQueueType type)
        {
            return _outgoMailQueues.GetOrAdd(type, (n) => new OutgoMailQueue<DBMailQueueType>(n));
        }

        public bool RemoveOutgoMailQueue(DBMailQueueType type)
        {
            var r = _outgoMailQueues.TryRemove(type, out var _);
            return r;
        }
    }
}
