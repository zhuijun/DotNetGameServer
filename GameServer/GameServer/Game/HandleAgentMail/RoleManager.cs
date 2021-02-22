﻿using GameServer.Common;
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
                case (int)ClientServerProto.MessageId.CtoSenterRoleRequestId:
                    OnEnterRoleRequest(mail);
                    break;
                default:
                    break;
            }
        }

        public void OnJoinGame(AgentGameProto.JoinGameRequest request, long clientId)
        {
        }

        public void OnLeaveGame(AgentGameProto.LeaveGameRequest request, long clientId)
        {
            if (_clientRoleDict.TryGetValue(clientId, out var roleId))
            {
                Items.Remove(roleId);
                _clientRoleDict.Remove(clientId);
            }
        }

        private void OnEnterRoleRequest(MailPacket mail)
        {
            //var request = ClientServerProto.CtoSEnterRoleRequest.Parser.ParseFrom(mail.Content);
            var user = ManagerMediator.UserManager.GetItem(mail.UserId);
            var dbRequest = new GameDBProto.EnterRoleRequest { UserId = mail.UserId, NickName = user.NickName };
            var dbMail = new MailPacket { Id = (int)GameDBProto.MessageId.EnterRoleRequestId, Content = dbRequest.ToByteArray(), Reserve = mail.Reserve, UserId = mail.UserId, ClientId = mail.ClientId };
            Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Role);
        }
    }
}
