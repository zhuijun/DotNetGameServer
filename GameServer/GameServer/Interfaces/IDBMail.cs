using ServicesCore.Common;

namespace GameServer.Interfaces
{
    interface IDBMail
    {
        public void OnDBMail(MailPacket mail);
    }
}
