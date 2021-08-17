using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MusicLib.Model.Config
{
    public class DeviceConfig : IConfig
    {
        public string GetPath() => Path.Combine(Utils.GetProgramPath(), "DeviceConfig.json");

        public string Type { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public void Load()
        {
            if (File.Exists(GetPath()))
            {
                var obj = JObject.Parse(File.ReadAllText(GetPath()));
                Type = (string)obj["Type"];
                Id = (string)obj["Id"];
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
