using AgentGameProto;
using GameServer.Common;
using GameServer.Interfaces;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class RoomManager : IAgentMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)ClientServerProto.MessageId.CtoScreateDeskRequestId:
                    OnCreateDeskRequest(mail);
                    break;
                default:
                    var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
                    var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
                    if (desk != null)
                    {
                        if (desk.GameLogic is IAgentMail agentMail)
                        {
                            agentMail.OnAgentMail(mail);
                        }
                    }
                    break;
            }
        }

        public void OnJoinGame(JoinGameRequest request, long clientId)
        {
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(clientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IAgentMail agentMail)
                {
                    agentMail.OnJoinGame(request, clientId);
                }
            }
        }

        public void OnLeaveGame(LeaveGameRequest request, long clientId)
        {
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(clientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IAgentMail agentMail)
                {
                    agentMail.OnLeaveGame(request, clientId);
                }
                desk.RemoveRole(roleId);
                if (desk.RoleCount() == 0)
                {
                    _room.RemoveDesk(desk.DeskId);
                }
                _room.RemoveRoleDesk(roleId);
            }
        }

        private void OnCreateDeskRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCCreateDeskReply();
            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var deskId = _room.GetRoleDesk(roleId);
            if (deskId == 0)
            {
                var game = _gameFactory.CreateGame(1);
                var desk = _room.CreateDesk(game, 3);
                deskId = desk.DeskId;
                _room.AddRoleDesk(roleId, deskId);

                stoc.DeskId = deskId;
            }
            else
            {
                stoc.Result.ErrorCode = 1;
                stoc.Result.ErrorInfo = "already on desk";
                stoc.DeskId = deskId;
            }
            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCcreateDeskReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }
    }
}
