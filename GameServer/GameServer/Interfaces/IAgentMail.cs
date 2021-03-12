using ServicesCore.Common;

namespace GameServer.Interfaces
{
    interface IAgentMail
    {
        public void OnAgentMail(MailPacket mail);
    }
}
