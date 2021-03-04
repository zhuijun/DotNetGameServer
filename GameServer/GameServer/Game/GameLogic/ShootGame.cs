using AgentGameProto;
using GameServer.Common;
using GameServer.Interfaces;
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

        public void OnJoinGame(JoinGameRequest request, long clientId)
        {
            //throw new NotImplementedException();
        }

        public void OnLeaveGame(LeaveGameRequest request, long clientId)
        {
            //throw new NotImplementedException();
        }
    }
}
