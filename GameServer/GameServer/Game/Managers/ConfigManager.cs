using GameServer.Common;
using GameServer.Interfaces;
using GameServer.Services;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public partial class ConfigManager : IManager, IStartUp
    {
        public ManagerMediator ManagerMediator { get; set; }
        public Dispatcher Dispatcher { get; set; }
        public IConfiguration Configuration { get; }

        public GameConfig.TestConfig TestConfig { get; } = new GameConfig.TestConfig();
        public WatermelonConfigProto.FruitConfig FruitConfig { get; set; }
        public WatermelonConfigProto.TruntableConfig TruntableConfig { get; set; }

        public ConfigManager(IConfiguration configuration)
        {
            Configuration = configuration;
            LoadConfig();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private void LoadConfig()
        {
            var configPath = Configuration.GetValue<string>("GameConfigPath");
            var jsonPath = Path.Combine(configPath, "Json");

            //测试配置
            var testJson = File.OpenRead(Path.Combine(jsonPath, "test.json"));
            using (JsonDocument document = JsonDocument.Parse(testJson))
            {
                JsonElement root = document.RootElement;
                foreach (JsonElement element in root.EnumerateArray())
                {
                    var item = GameConfig.TestItem.Parser.ParseJson(element.GetRawText());
                    TestConfig.Items.Add(item.Id, item);
                }
            }

            //Console.WriteLine(TestConfig);
        }

        public void OnStartUp()
        {
            var dbRequest = new GameDBProto.LoadConfigRequest { };
            var dbMail = new MailPacket
            {
                Id = (int)GameDBProto.MessageId.LoadConfigRequestId,
                Content = dbRequest.ToByteArray(),
            };
            Dispatcher.WriteDBMail(dbMail, DBMailQueueType.Other);
        }
    }
}
