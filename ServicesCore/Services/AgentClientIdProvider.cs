using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServicesCore.Services
{
    public class AgentClientIdProvider
    {
        private readonly ConcurrentDictionary<long, long> _userClientIdMap = new ConcurrentDictionary<long, long>();
        private long _clientIdSeed = 0;

        public long CreateClientId()
        {
            return Interlocked.Increment(ref _clientIdSeed);
        }

        public void SetUserClientId(long userId, long ClientId)
        {
            _userClientIdMap[userId] = ClientId;
        }

        public long GetUserClientId(long userId)
        {
            _userClientIdMap.TryGetValue(userId, out var clientId);
            return clientId;
        }

        public bool RemoveUserClientId(long userId)
        {
            return _userClientIdMap.TryRemove(userId, out var _);
        }
    }
}
