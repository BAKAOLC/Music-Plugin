using System;

namespace MusicLib.Model.Music
{
    public class SongSearchResult
    {
        public string Id { get; }

        public string Name { get; }

        public BaseArtist[] Artists { get; }

        public BaseAlbum Album { get; }

        public DateTime PublishTime { get; }

        public TimeSpan Duration { get; }

        public string Url { get; }

        public SongSearchResult(string id, string name, BaseArtist[] artists, BaseAlbum album, DateTime time, TimeSpan duration, string url)
        {
            Id = id;
            Name = name;
            Artists = artists;
            Album = album;
            PublishTime = time;
            Duration = duration;
            Url = url;
        }
    }
}
