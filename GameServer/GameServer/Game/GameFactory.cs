using GameServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class GameFactory
    {
        public ManagerMediator ManagerMediator { get; }

        public GameFactory(ManagerMediator managerMediator)
        {
            ManagerMediator = managerMediator;
        }

        public object CreateGame(GameType type)
        {
            switch (type)
            {
                case GameType.Watermelon:
                    return new WatermelonGame(ManagerMediator);
                default:
                    break;
            }
            return null;
        }
    }
}
