using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentServer.Hubs
{
    public class NetPacket
    {
        public int Id { get; init; }
        public string Content { get; init; }
        public long Reserve { get; init; }
    }
}
