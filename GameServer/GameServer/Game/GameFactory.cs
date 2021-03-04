using GameServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class GameFactory
    {
        public object CreateGame(GameType type, ManagerMediator managerMediator)
        {
            switch (type)
            {
                case GameType.Watermelon:
                    return new WatermelonGame(managerMediator);
                default:
                    break;
            }
            return null;
        }
    }
}
