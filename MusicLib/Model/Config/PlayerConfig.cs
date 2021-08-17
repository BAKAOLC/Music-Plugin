using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;

namespace MusicLib.Model.Config
{
    public class PlayerConfig : IConfig, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string GetPath() => Path.Combine(Utils.GetProgramPath(), "PlayerConfig.json");

        int _volume = 100;
        public int Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Volume"));
                Save();
            }
        }

        bool _outputPlayerInfo = false;
        public bool OutputPlayerInfo
        {
            get => _outputPlayerInfo;
            set
            {
                _outputPlayerInfo = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OutputPlayerInfo"));
                Save();
            }
        }

        const string DefaultPlayerOutputFormatString = "当前正在播放:\r\n{1:Id} - {1:Title} - {1:Author}  {1:Api}\r\n{1:current} / {1:length}\r\n播放列表中还有{0}首歌曲";
        string _playerOutputFormatString = DefaultPlayerOutputFormatString;
        public string PlayerOutputFormatString
        {
            get => _playerOutputFormatString;
            set
            {
                _playerOutputFormatString = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PlayerOutputFormatString"));
                Save();
            }
        }

        public void Load()
        {
            if (File.Exists(GetPath()))
            {
                var obj = JObject.Parse(File.ReadAllText(GetPath()));
                Volume = Math.Max(0, Math.Min((int)(obj["Volume"] ?? 100), 100));
                OutputPlayerInfo = (bool)(obj["OutputPlayerInfo"] ?? false);
                PlayerOutputFormatString = (string)(obj["PlayerOutputFormatString"] ?? DefaultPlayerOutputFormatString);
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
