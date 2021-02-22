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
                case (int)ClientServerProto.MessageId.CtoSjoinDeskRequestId:
                    OnJoinDeskRequest(mail);
                    break;
                case (int)ClientServerProto.MessageId.CtoSleaveDeskRequestId:
                    OnLeaveDeskRequest(mail);
                    break;
                default:
                    break;
            }

            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var desk = _room.GetDesk(_room.GetRoleDesk(roleId));
            if (desk != null)
            {
                if (desk.GameLogic is IAgentMail agentMail)
                {
                    agentMail.OnAgentMail(mail);
                }
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
                desk.AddRole(roleId);
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

        private void OnJoinDeskRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCJoinDeskReply();

            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var ctos = ClientServerProto.CtoSJoinDeskRequest.Parser.ParseFrom(mail.Content);
            var deskId = ctos.DeskId;
            var desk = _room.GetDesk(deskId);
            if (desk != null)
            {
                desk.AddRole(roleId);
                _room.AddRoleDesk(roleId, deskId);
            }
            else
            {
                stoc.Result.ErrorCode = 1;
                stoc.Result.ErrorInfo = "not find desk";
            }

            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCjoinDeskReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }

        private void OnLeaveDeskRequest(MailPacket mail)
        {
            var stoc = new ClientServerProto.StoCLeaveDeskReply();

            var roleId = ManagerMediator.RoleManager.GetRoleIdByClientId(mail.ClientId);
            var deskId = _room.GetRoleDesk(roleId);
            if (deskId != 0)
            {
                var desk = _room.GetDesk(deskId);
                if (desk != null)
                {
                    if (desk.GameLogic is IAgentMail agentMail)
                    {
                        var request = new LeaveGameRequest { UserId = mail.UserId };
                        agentMail.OnLeaveGame(request, mail.ClientId);
                    }
                    desk.RemoveRole(roleId);
                    if (desk.RoleCount() == 0)
                    {
                        _room.RemoveDesk(desk.DeskId);
                    }
                    _room.RemoveRoleDesk(roleId);
                }
            }
            else
            {
                stoc.Result.ErrorCode = 1;
                stoc.Result.ErrorInfo = "not on desk";
            }

            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCleaveDeskReplyId,
                Content = stoc.ToByteArray(),
                Reserve = mail.Reserve,
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }
    }
}
