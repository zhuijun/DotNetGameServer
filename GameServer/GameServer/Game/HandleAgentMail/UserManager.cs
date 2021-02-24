using GameServer.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class UserManager : IAgentMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            _logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                default:
                    break;
            }
        }

        public void OnJoinGame(AgentGameProto.JoinGameRequest request, long clientId)
        {
            var user = new User { UserId = request.UserId, NickName = request.NickName, HeadIcon = request.HeadIcon };
            AddItem(user.UserId, user);
        }

        public void OnLeaveGame(AgentGameProto.LeaveGameRequest request, long clientId)
        {
            RemoveItem(request.UserId);
        }
    }
}
