using AgentGameProto;
using GameServer.Common;
using GameServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class WatermelonGame : IAgentMail, IDBMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)WatermelonGameProto.MessageId.CtoSdropBoxRequestId:
                    AgentDropBoxRequest(mail);
                    break;
                case (int)WatermelonGameProto.MessageId.CtoScombineFruitRequestId:
                    AgentCombineFruitRequest(mail);
                    break;
                default:
                    break;
            }
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

        private void AgentDropBoxRequest(MailPacket mail)
        {

        }

        private void AgentCombineFruitRequest(MailPacket mail)
        {

        }
    }
}
