using Microsoft.Extensions.DependencyInjection;
using ServicesCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesCore
{
    public static class ServiceCollectionExtensions
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
