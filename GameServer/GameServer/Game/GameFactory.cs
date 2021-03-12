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
