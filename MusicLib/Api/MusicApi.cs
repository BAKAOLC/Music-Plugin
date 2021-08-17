using MusicLib.Model.Music;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MusicLib.Api
{
    public class MusicApi : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        ApiType _api = ApiType.NeteaseCloudMusic;
        public ApiType ApiType
        {
            get => _api;
            set
            {
                _api = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ApiType"));
            }
        }

        public async Task<SongSearchResult[]> SearchSong(string keyword)
        {
            switch (ApiType)
            {
                case ApiType.NeteaseCloudMusic:
                    return await new NeteaseCloudMusicApi().SearchSong(keyword);
                default:
                    break;
            }
            return null;
        }

        public async Task<SongDetail> GetSongDetail(string id)
        {
            switch (ApiType)
            {
                case ApiType.NeteaseCloudMusic:
                    return await new NeteaseCloudMusicApi().GetSongDetail(id);
                default:
                    break;
            }
            return null;
        }

        public async Task<SongUrl> GetSongUrl(string id, int br = 999000)
        {
            switch (ApiType)
            {
                case ApiType.NeteaseCloudMusic:
                    return await new NeteaseCloudMusicApi().GetSongUrl(id, br);
                default:
                    break;
            }
            return new SongUrl();
        }

        public async Task<NetMusic> GetMusic(string keyword)
        {
            var results = await SearchSong(keyword);
            var result = results.FirstOrDefault();
            if (result == null)
                return null;
            var detail = await GetSongDetail(result.Id);
            if (detail == null)
                return null;
            var url = await GetSongUrl(detail.Id, 128000);
            if (url.Id == detail.Id && url.Id == result.Id)
                return new NetMusic(detail, url, ApiType);
            else
                return null;
        }
    }
}
