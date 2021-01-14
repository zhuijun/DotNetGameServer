using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class Mail
    {
        public Mail(long clientId, int id, byte[] content)
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
