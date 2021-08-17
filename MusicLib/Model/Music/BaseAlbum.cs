using Newtonsoft.Json.Linq;

namespace MusicLib.Model.Music
{
    public struct BaseAlbum
    {
        public string Id { get; }
        public string Name { get; }
        public string Pic { get; }

        public BaseAlbum(string id, string name, string pic)
        {
            Id = id;
            Name = name;
            Pic = pic;
        }
        public BaseAlbum(JToken data) : this((string)data["id"], (string)data["name"], (string)data["picUrl"]) { }

        public string GetPicUrl(int width, int height)
            => Pic + $"?param={width}y{height}";
    }
}
