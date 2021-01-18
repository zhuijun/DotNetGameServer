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
    public partial class RoleManager : IAgentMail
    {
        private TimeoutLinker linker;
        public void OnAgentMail(MailPacket mail)
        {
            _logger.LogDebug(mail.ToString());

            switch (mail.Id)
            {
                case 1:
                    if (linker == null)
                    {
                        Action fun()
                        {
                            int i = 0;
                            return () =>
                            {
                                var mm = new MailPacket { Id = mail.Id, Content = mail.Content, Reserve = mail.Reserve, ClientId = mail.ClientId };
                                Dispatcher.WriteAgentMail(mm);
                                Dispatcher.WriteDBMail(mail, Services.DBMailQueueType.Role);
                                if (++i == 10)
                                {
                                    linker.Valid = false;
                                    linker = null;
                                }
                            };
                        }
                        linker = Dispatcher.QuickTimer.SetTimeoutWithLinker(fun(), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
