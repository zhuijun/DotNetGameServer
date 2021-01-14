using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Services
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
}
