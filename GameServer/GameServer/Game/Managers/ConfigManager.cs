using GameServer.Interfaces;
using GameServer.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameServer.Game
{
    public class ConfigManager : IManager
    {
        public ManagerMediator ManagerMediator { get; set; }
        public Dispatcher Dispatcher { get; set; }
        public IConfiguration Configuration { get; }

        public GameConfig.TestConfig TestConfig { get; } = new GameConfig.TestConfig();

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
            var testJson = File.ReadAllText(Path.Combine(jsonPath, "test.json"));
            using (JsonDocument document = JsonDocument.Parse(testJson))
            {
                JsonElement root = document.RootElement;
                foreach (JsonElement item in root.EnumerateArray())
                {
                    var config = GameConfig.TestItem.Parser.ParseJson(item.GetRawText());
                    TestConfig.Items.Add(config.Id, config);
                }
            }

            Console.WriteLine(TestConfig);
        }
    }
}
