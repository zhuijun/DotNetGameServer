using GameServer.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoleManager : IAgentMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            _logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                case (int)ClientServerProto.MessageID.CtoSenterRoleRequestId:
                    OnEnterRoleRequest(mail);
                    break;
                default:
                    break;
            }
        }

        public void OnJoinGame(AgentGameProto.JoinGameRequest request)
        {
            //throw new NotImplementedException();
        }

        public void OnLeaveGame(AgentGameProto.LeaveGameRequest request)
        {
            //throw new NotImplementedException();
        }

        private void OnEnterRoleRequest(MailPacket mail)
        {
            //var request = ClientServerProto.CtoSEnterRoleRequest.Parser.ParseFrom(mail.Content);
            var dbRequest = new GameDBProto.EnterRoleRequest { UserId = mail.UserId };
            var dbMail = new MailPacket { Id = (int)GameDBProto.MessageID.EnterRoleRequestId, Content = dbRequest.ToByteArray(), Reserve = mail.Reserve, UserId = mail.UserId, ClientId = mail.ClientId };
            Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Role);
        }
    }
}
