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
                case (int)AgentGameProto.MessageID.AtoGjoinGameRequestId:
                    {
                        OnAtoGJoinGameRequest(mail);
                    }
                    break;
                default:
                    break;
            }
        }

        void OnAtoGJoinGameRequest(MailPacket mail)
        {
            var request = AgentGameProto.AtoGJoinGameRequest.Parser.ParseFrom(mail.Content);
            _logger.LogInformation(request.ToString());

            var user = new User { UserID = request.UserID, NickName = request.NickName };
            AddItem(user.UserID, user);

            //var mm = new MailPacket { Id = mail.Id, Content = mail.Content, Reserve = mail.Reserve, ClientId = mail.ClientId };
            //Dispatcher.WriteAgentMail(mm);
            //Dispatcher.WriteDBMail(mail, Services.DBMailQueueType.Role);
        }
    }
}
