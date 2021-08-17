using MusicLib;
using MusicLib.Model.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace MusicPlugin
{
    public class PluginConfig : IConfig
    {
        public string GetPath() => Path.Combine(Utils.GetProgramPath(), "PluginConfig.json");

        public bool IsEnable { get; set; } = false;

        public void Load()
        {
            if (File.Exists(GetPath()))
            {
                var obj = JObject.Parse(File.ReadAllText(GetPath()));
                IsEnable = (bool)(obj["IsEnable"] ?? false);
            }
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(GetPath()));
            File.WriteAllText(GetPath(), JsonConvert.SerializeObject(this));
        }

        public void Delete()
            => File.Delete(GetPath());
    }
}
