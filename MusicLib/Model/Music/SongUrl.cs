namespace MusicLib.Model.Music
{
    public struct SongUrl
    {
        public string Id { get; }

        public int Br { get; }

        public long Size { get; }

        public string Type { get; }

        public string Url { get; }

        public SongUrl(string id, int br, long size, string type, string url)
        {
            Id = id;
            Br = br;
            Size = size;
            Type = type;
            Url = url;
        }
    }
}
