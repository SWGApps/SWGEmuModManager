using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SWGEmuModManager.Models
{
    public class ConfigFile
    {
        static readonly string _configFile = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "SWGEmuModManager/config.json");

        [JsonPropertyName("swgDirectory")]
        public string? SwgDirectory { get; set; }

        [JsonPropertyName("installedMods")]
        public List<int>? InstalledMods { get; set; }

        public static async Task GenerateNewConfig(bool deleteCurrent = false)
        {
            if (deleteCurrent) File.Delete(path: _configFile);

            if (!Directory.Exists(Path.GetDirectoryName(path: _configFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path: _configFile)!);
            }

            if (File.Exists(path: _configFile)) return;

            ConfigFile config = new()
            {
                SwgDirectory = "",
                InstalledMods = new List<int>()
            };

            await using StreamWriter sw = new(path: _configFile);

            try
            {
                await sw.WriteAsync(JsonSerializer.Serialize(config, 
                    new JsonSerializerOptions() { WriteIndented = true }));
            } catch { }
        }

        public static void SetConfig(ConfigFile config)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path: _configFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path: _configFile)!);
            }

            using StreamWriter sw = new(path: _configFile);

            sw.Write(JsonSerializer.Serialize(config, 
                new JsonSerializerOptions() { WriteIndented = true }));
        }

        public static ConfigFile? GetConfig()
        {
            using StreamReader sr = new(path: _configFile, detectEncodingFromByteOrderMarks: true);

            string data = sr.ReadToEnd();

            return JsonSerializer.Deserialize<ConfigFile>(data);
        }
    }
}
