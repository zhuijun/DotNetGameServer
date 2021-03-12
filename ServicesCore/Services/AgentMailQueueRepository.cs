#region Copyright notice and license

// Copyright 2019 The gRPC Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Collections.Concurrent;
using System.Threading;

namespace ServicesCore.Services
{
    public class AgentMailQueueRepository
    {
        private readonly ConcurrentDictionary<long, OutgoMailQueue<long>> _outgoMailQueues = new ConcurrentDictionary<long, OutgoMailQueue<long>>();

        private readonly IncomeMailQueue _incomeMailQueue = new IncomeMailQueue();
        public IncomeMailQueue GetIncomeMailQueue()
        {
            return _incomeMailQueue;
        }

        public OutgoMailQueue<long> GetOrAddOutgoMailQueue(long clientId)
        {
            return _outgoMailQueues.GetOrAdd(clientId, (n) => new OutgoMailQueue<long>(n));
        }

        public OutgoMailQueue<long> TryGetOutgoMailQueue(long clientId)
        {
            _outgoMailQueues.TryGetValue(clientId, out var outgoMailQueue);
            return outgoMailQueue;
        }

        public bool TryRemoveOutgoMailQueue(long clientId)
        {
            var r = _outgoMailQueues.TryRemove(clientId, out var _);
            return r;
        }
    }
}
