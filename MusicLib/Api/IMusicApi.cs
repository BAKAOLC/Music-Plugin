using MusicLib.Model.Music;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace MusicLib.Api
{
    public interface IMusicApi
    {
        public SongSearchResult ReadSearchResult(JToken data);

        public SongDetail ReadSongDetail(JToken data);

        public SongUrl ReadSongUrl(JToken data);

        public Task<SongSearchResult[]> SearchSong(string keyword);

        public Task<SongDetail> GetSongDetail(string id);

        public Task<SongUrl> GetSongUrl(string id, int br = 999000);
    }
}
