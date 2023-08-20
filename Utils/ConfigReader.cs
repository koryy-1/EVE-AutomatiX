using EVE_AutomatiX.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EVE_AutomatiX.Utils
{
    public static class ConfigReader
    {
        public static List<string> GetConfigPaths()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json")
                .Where(file => file.Contains("config")).ToList();
        }

        public static List<string> GetNickNameList(List<string> jsonFilePaths)
        {
            List<string> nickNameList = new List<string>();
            foreach (var filePath in jsonFilePaths)
            {
                nickNameList.Add(Path.GetFileName(filePath).Replace("-config.json", ""));
            }
            return nickNameList;
        }

        public static Config GetConfigByNickName(string nick)
        {
            if (!File.Exists($"{nick}-config.json"))
            {
                return null;
            }
            string json = File.ReadAllText($"{nick}-config.json");
            Config config = JsonSerializer.Deserialize<Config>(json);
            return config;
            //return botConfigs.Find(config => config._nickname == nick);
        }

        public static void CreateNewConfig()
        {
            string config = JsonSerializer.Serialize(new Config());
            File.WriteAllText($"nickname-config.json", config);
        }

        public static void UpdateConfig(string nickName, Config config)
        {
            // update config in file
            string configString = JsonSerializer.Serialize(config);
            // todo get nickname for file name from another way
            File.WriteAllText($"{nickName}-config.json", configString);
            //int index = botConfigs.FindIndex(item => item._nickname == config._nickname);
            //botConfigs[index] = config;
        }

        public static void RenameConfig(string oldNickName, string newNickName)
        {
            File.Move($"{oldNickName}-config.json", $"{newNickName}-config.json");
        }
    }
}
