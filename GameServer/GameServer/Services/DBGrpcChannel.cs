using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

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
