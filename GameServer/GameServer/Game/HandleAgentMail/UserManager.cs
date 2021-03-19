using AgentGameProto;
using ServicesCore.Common;
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
    public partial class UserManager : IAgentMail, IInnerMail
    {
        public void OnAgentMail(MailPacket mail)
        {
            //_logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                case (int)ClientServerProto.MessageId.CtoStestNetwordDelayRequestId:
                    OnTestNetwordDelayRequest(mail);
                    break;
                default:
                    break;
            }
        }

        public void OnEnterGame(AgentGameProto.EnterGameRequest request, long clientId)
        {
            var user = new User { UserId = request.UserId, NickName = request.NickName, HeadIcon = request.HeadIcon };
            AddItem(user.UserId, user);

            //var timeoutLinker = Dispatcher.QuickTimer.SetTimeoutWithLinker(() => {
            //    var ticks = Dispatcher.TicksProvider.TicksCache;
            //    _userTick[clientId] = ticks;

            //    var stoc = new ClientServerProto.StoCBeginTestNetworkDelay();
            //    Dispatcher.WriteAgentMail(new MailPacket
            //    {
            //        Id = (int)ClientServerProto.MessageId.StoCbeginTestNetworkDelayId,
            //        Content = stoc.ToByteArray(),
            //        UserId = request.UserId,
            //        ClientId = clientId
            //    });
            //}, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            //_userTimeoutLinker.Add(clientId, timeoutLinker);
        }

        public void OnLeaveGame(AgentGameProto.LeaveGameRequest request, long clientId)
        {
            RemoveItem(request.UserId);

            var timeoutLinker = _userTimeoutLinker.GetValueOrDefault(clientId);
            if (timeoutLinker != null)
            {
                timeoutLinker.Valid = false;
                _userTimeoutLinker.Remove(clientId);
            }

            _userTick.Remove(clientId);
        }

        public void BeforeLeaveGame(BeforeLeaveGameRequest request, long clientId)
        {
            //throw new NotImplementedException();
        }

        private void OnTestNetwordDelayRequest(MailPacket mail)
        {
            var ticks = _userTick[mail.ClientId];
            var currentTicks = Dispatcher.TicksProvider.TicksCache;
            var stoc = new ClientServerProto.StoCTestNetworkDelayReply { CurrentTicks = currentTicks, DelayTicks = currentTicks - ticks};
            Dispatcher.WriteAgentMail(new MailPacket
            {
                Id = (int)ClientServerProto.MessageId.StoCtestNetworkDelayReplyId,
                Content = stoc.ToByteArray(),
                UserId = mail.UserId,
                ClientId = mail.ClientId
            });
        }
    }
}
