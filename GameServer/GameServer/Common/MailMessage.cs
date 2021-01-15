using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Common
{
    public class MailMessage
    {
        public MailMessage(long clientId, int id, byte[] content)
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
