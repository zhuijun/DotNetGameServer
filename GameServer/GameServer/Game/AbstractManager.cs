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
        private Dispatcher _dispatcher;
        private ManagerMediator _mediator;

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
                return _dispatcher;
            }
            set
            {
                _dispatcher = value;
            }
        }

    }
}
