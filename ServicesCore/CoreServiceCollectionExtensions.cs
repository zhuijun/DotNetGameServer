using Microsoft.Extensions.DependencyInjection;
using ServicesCore.Services;

namespace ServicesCore
{
    public static class CoreServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesCore(this IServiceCollection services)
        {
            services.AddSingleton<AgentMailQueueRepository>();
            services.AddSingleton<DBMailQueueRepository>();
            services.AddSingleton<Dispatcher>();
            services.AddSingleton<MailDispatcher>();
            services.AddSingleton<TicksProvider>();
            services.AddSingleton<QuickTimer>();
            services.AddSingleton<AgentClientIdProvider>();
            return services;
        }
    }
}
