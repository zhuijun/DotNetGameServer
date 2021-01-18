using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentServer.Services
{
    public class GameGrpcChannel
    {
        public GrpcChannel Channel { get; set; }

        public GameGrpcChannel(IConfiguration configuration)
        {
            var address = configuration.GetValue<string>("GameServerURL");
            Channel = GrpcChannel.ForAddress(address);
        }
    }
}
