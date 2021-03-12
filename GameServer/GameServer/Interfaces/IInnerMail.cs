using ServicesCore.Common;

namespace GameServer.Interfaces
{
    interface IInnerMail
    {
        public sealed void OnInnerMailEx(MailPacket mail)
        {
            switch (mail.Id)
            {
                case (int)AgentGameProto.MessageId.EnterGameRequestId:
                    {
                        var request = AgentGameProto.EnterGameRequest.Parser.ParseFrom(mail.Content);
                        OnEnterGame(request, mail.ClientId);
                    }
                    break;
                case (int)AgentGameProto.MessageId.BeforeLeaveGameRequestId:
                    {
                        var request = AgentGameProto.BeforeLeaveGameRequest.Parser.ParseFrom(mail.Content);
                        BeforeLeaveGame(request, mail.ClientId);
                    }
                    break;
                case (int)AgentGameProto.MessageId.LeaveGameRequestId:
                    {
                        var request = AgentGameProto.LeaveGameRequest.Parser.ParseFrom(mail.Content);
                        OnLeaveGame(request, mail.ClientId);
                    }
                    break;
                default:
                    OnInnerMail(mail);
                    break;
            }
        }

        public void OnInnerMail(MailPacket mail)
        {

        }

        public void OnEnterGame(AgentGameProto.EnterGameRequest request, long clientId);
        public void OnLeaveGame(AgentGameProto.LeaveGameRequest request, long clientId);
        public void BeforeLeaveGame(AgentGameProto.BeforeLeaveGameRequest request, long clientId);
    }
}
