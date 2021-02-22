using AgentGameProto;
using GameServer.Common;
using GameServer.Interfaces;
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
            }
        }
    }
}
