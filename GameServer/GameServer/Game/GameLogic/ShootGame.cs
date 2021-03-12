using AgentGameProto;
using GameServer.Interfaces;
using ServicesCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class ShootGame : IAgentMail, IDBMail
    {
        public void BeforeLeaveGame(BeforeLeaveGameRequest request, long clientId)
        {
            //throw new NotImplementedException();
        }

        public void OnAgentMail(MailPacket mail)
        {
            //throw new NotImplementedException();
        }

        public void OnDBMail(MailPacket mail)
        {
            //throw new NotImplementedException();
        }
    }
}
