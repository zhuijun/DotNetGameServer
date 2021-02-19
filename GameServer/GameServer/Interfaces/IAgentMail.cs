using GameServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Interfaces
{
    interface IAgentMail
    {
        public sealed void OnAgentMailEx(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)AgentGameProto.MessageID.AtoGjoinGameRequestId:
                    {
                        var request = AgentGameProto.AtoGJoinGameRequest.Parser.ParseFrom(mail.Content);
                        OnJoinGame(request);
                    }
                    break;
                case (int)AgentGameProto.MessageID.AtoGleaveGameRequestId:
                    {
                        var request = AgentGameProto.AtoGLeaveGameRequest.Parser.ParseFrom(mail.Content);
                        OnLeaveGame(request);
                    }
                    break;
                default:
                    OnAgentMail(mail);
                    break;
            }
        }

        public void OnAgentMail(MailPacket mail);

        public void OnJoinGame(AgentGameProto.AtoGJoinGameRequest request);
        public void OnLeaveGame(AgentGameProto.AtoGLeaveGameRequest request);
    }
}
