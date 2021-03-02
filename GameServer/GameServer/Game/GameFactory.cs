using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class GameFactory
    {
        public object CreateGame(GameType type)
        {
            switch (type)
            {
                case GameType.Watermelon:
                    return new WatermelonGame();
                default:
                    break;
            }
            return null;
        }
    }
}
