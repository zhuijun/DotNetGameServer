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

using GameServer.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace GameServer.Services
{
    public class IncomeMailQueue
    {
        private readonly Channel<MailPacket> _mailChannel;
        private int _totalMailCount;

        public IncomeMailQueue()
        {
            _mailChannel = Channel.CreateUnbounded<MailPacket>();
            _totalMailCount = 0;
        }

        public bool TryReadMail([NotNullWhen(true)] out MailPacket? mail)
        {
            if (_mailChannel.Reader.TryRead(out mail))
            {
                Interlocked.Decrement(ref _totalMailCount);
                return true;
            }
            return false;
        }
        
        public async ValueTask WriteAsync(MailPacket mail)
        {
            await _mailChannel.Writer.WriteAsync(mail);
            Interlocked.Increment(ref _totalMailCount);
        }
    }
}
