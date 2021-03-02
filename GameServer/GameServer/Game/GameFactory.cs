using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class GameFactory
    {
        public object CreateGame(int type)
        {
            switch (type)
            {
                case 1:
                    return new WatermelonGame();
                default:
                    break;
            }
            return null;
        }
    }
}
