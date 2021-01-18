using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class DBGrpcChannel
    {
        public GrpcChannel Channel { get; set; }

        public DBGrpcChannel(IConfiguration configuration)
        {
            var address = configuration.GetValue<string>("DBServerURL");
            Channel = GrpcChannel.ForAddress(address);
        }
    }
}
