using GameServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Interfaces
{
    interface IAgentMail
    {
        public void OnAgentMail(MailPacket mail);
    }
}
