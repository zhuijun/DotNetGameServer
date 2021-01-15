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
        private IServiceProvider _services;
        private ManagerMediator _mediator;

        public void SetServiceProvider(IServiceProvider services)
        {
            _services = services;
        }

        public ManagerMediator ManagerMediator
        {
            get 
            {
                return _mediator; 
            }
            set
            {
                _mediator = value;
            }
        }

        public Dispatcher Dispatcher
        {
            get
            {
                return _services.GetRequiredService<Dispatcher>();
            }
        }

    }
}
