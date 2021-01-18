using GameServer.Game;
using GameServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Interfaces
{
    public interface IManager : IDisposable
    {
        public ManagerMediator ManagerMediator { get; set; }

        public Dispatcher Dispatcher { get; set; }
    }
}
