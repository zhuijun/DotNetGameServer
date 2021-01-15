using GameServer.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameServiceCollectionExtensions
    {
        public static IServiceCollection AddGame(this IServiceCollection services)
        {
            services.AddSingleton<ManagerMediator>();
            services.AddSingleton<RoleManager>();


            return services;
        }
    }
}
