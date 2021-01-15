using GameServer.Interfaces;
using GameServer.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public abstract class AbstractManager : IManager
    {
        public ManagerMediator ManagerMediator { get; set; }

        public Dispatcher Dispatcher { get; set; }

    }
}
