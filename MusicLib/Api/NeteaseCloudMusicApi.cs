using MusicLib.Model.Music;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseApi = NeteaseCloudMusicApi.CloudMusicApi;
using Providers = NeteaseCloudMusicApi.CloudMusicApiProviders;

namespace MusicLib.Api
{
    public class NeteaseCloudMusicApi : IMusicApi
    {
        public SongSearchResult ReadSearchResult(JToken data)
        {
            var id = (string)data["id"];
            var name = (string)data["name"];
            List<BaseArtist> artistList = new List<BaseArtist>();
            foreach (var a in (JArray)data["artists"])
                artistList.Add(new BaseArtist(a));
            var artists = artistList.ToArray();
            var album = new BaseAlbum((string)data["album"]["id"], (string)data["album"]["name"], string.Empty);
            var publish = Utils.GetDateTime((double)data["album"]["publishTime"] / 1000);
            var duration = new TimeSpan(0, 0, (int)data["duration"]);
            var url = "https://music.163.com/#/song?id=" + id;
            return new SongSearchResult(id, name, artists, album, publish, duration, url);
        }

        public SongDetail ReadSongDetail(JToken data)
        {
            var id = (string)data["id"];
            var name = (string)data["name"];
            List<BaseArtist> artistList = new List<BaseArtist>();
            foreach (var a in (JArray)data["ar"])
                artistList.Add(new BaseArtist(a));
            var artists = artistList.ToArray();
            var album = new BaseAlbum(data["al"]);
            var publish = Utils.GetDateTime((double)data["publishTime"] / 1000);
            var duration = new TimeSpan(0, 0, (int)data["dt"]);
            var url = "https://music.163.com/#/song?id=" + id;
            return new SongDetail(id, name, artists, album, publish, duration, url);
        }

        public SongUrl ReadSongUrl(JToken data)
            => new SongUrl((string)data["id"], (int)data["br"], (long)data["size"], (string)data["type"], (string)data["url"]);

        public async Task<SongSearchResult[]> SearchSong(string keyword)
        {
            bool success;
            JObject json;
            var api = new BaseApi();
            (success, json) = await api.RequestAsync(Providers.Search, new Dictionary<string, object>()
            {
                { "keywords", keyword }
            });
            if (!success)
                return null;
            try
            {
                List<SongSearchResult> result = new List<SongSearchResult>();
                foreach (var s in (JArray)json["result"]["songs"])
                    result.Add(ReadSearchResult(s));
                return result.ToArray();
            }
            catch
            { }
            return null;
        }

        public async Task<SongDetail> GetSongDetail(string id)
        {
            bool success;
            JObject json;
            var api = new BaseApi();
            (success, json) = await api.RequestAsync(Providers.SongDetail, new Dictionary<string, object>()
            {
                { "ids", id }
            });
            if (!success)
                return null;
            return ReadSongDetail(json["songs"][0]);
        }

        public async Task<SongUrl> GetSongUrl(string id, int br = 999000)
        {
            bool success;
            JObject json;
            var api = new BaseApi();
            (success, json) = await api.RequestAsync(Providers.SongUrl, new Dictionary<string, object>()
            {
                { "id", id },
                { "br", br.ToString() }
            });
            if (!success)
                return new SongUrl();
            return ReadSongUrl(json["data"][0]);
        }
    }
}
