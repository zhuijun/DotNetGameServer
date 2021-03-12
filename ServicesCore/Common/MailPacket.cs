using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicesCore.Common
{
    public class MailPacket
    {
        public int Id { get; init; }
        public byte[] Content { get; init; }
        public long Reserve { get; init; }
        public long ClientId { get; init; }
        public long UserId { get; init; }
    }
}
