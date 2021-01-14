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

namespace GameServer.Services
{
    public class MailQueueRepository
    {
        private readonly ConcurrentDictionary<long, OutcomeMailQueue> _outcomeMailQueues = new ConcurrentDictionary<long, OutcomeMailQueue>();
        private long _clientIdSeed = 0;

        private readonly IncomeMailQueue _incomeMailQueue = new IncomeMailQueue();
        public IncomeMailQueue GetIncomeMailQueue()
        {
            return _incomeMailQueue;
        }

        public OutcomeMailQueue GetOutcomeMailQueue(long clientId)
        {
            return _outcomeMailQueues.GetOrAdd(clientId, (n) => new OutcomeMailQueue(n));
        }

        public bool RemoveOutcomeMailQueue(long clientId)
        {
            return _outcomeMailQueues.TryRemove(clientId, out var _);
        }

        public long CreateClientId()
        {
            return Interlocked.Increment(ref _clientIdSeed);
        }

    }
}
