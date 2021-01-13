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
#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Mail;

namespace GameServer
{
    public class Mail
    {
        public Mail(int id, byte[] content)
        {
            Id = id;
            Content = content;
        }

        public int Id { get; }
        public byte[] Content { get; }
    }

    public class MailQueue
    {
        private readonly Channel<Mail> _incomingMail;
        private int _totalMailCount;

        public string Name { get; }
        //public event Func<(int totalCount, int newCount, MailboxMessage.Types.Reason reason), Task>? Changed;

        public MailQueue(string name)
        {
            Name = name;
            _incomingMail = Channel.CreateUnbounded<Mail>();
            _totalMailCount = 0;
        }

        public bool TryReadMail([NotNullWhen(true)] out Mail? message)
        {
            if (_incomingMail.Reader.TryRead(out message))
            {
                Interlocked.Decrement(ref _totalMailCount);

                return true;
            }

            return false;
        }
        
        public async ValueTask WriteAsync(Mail message)
        {
            await _incomingMail.Writer.WriteAsync(message);
            Interlocked.Increment(ref _totalMailCount);
        }

    }
}
