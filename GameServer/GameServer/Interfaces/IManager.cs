using GameServer.Game;
using ServicesCore.Services;
using System;

namespace GameServer.Interfaces
{
    public interface IManager : IDisposable
    {
        public ManagerMediator ManagerMediator { get; set; }

        public Dispatcher Dispatcher { get; set; }

    }
}
