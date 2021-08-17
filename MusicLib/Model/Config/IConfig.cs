namespace MusicLib.Model.Config
{
    public interface IConfig
    {
        public string GetPath();

        public void Load();

        public void Save();

        public void Delete();
    }
}
