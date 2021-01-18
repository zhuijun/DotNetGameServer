using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Common
{
    public class MailPacket
    {
        public MailPacket(long clientId, int id, byte[] content)
        {
            ClientId = clientId;
            Id = id;
            Content = content;
        }

        public long ClientId { get; }
        public int Id { get; }
        public byte[] Content { get; }
    }
}
