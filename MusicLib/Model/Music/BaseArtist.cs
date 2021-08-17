using Newtonsoft.Json.Linq;

namespace MusicLib.Model.Music
{
    public struct BaseArtist
    {
        public long Id { get; }
        public string Name { get; }

        public BaseArtist(long id, string name)
        {
            Id = id;
            Name = name;
        }
        public BaseArtist(JToken data) : this((long)data["id"], (string)data["name"]) { }

        public override string ToString()
            => Name;
    }
}
