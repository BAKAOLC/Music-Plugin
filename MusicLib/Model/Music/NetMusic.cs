using Downloader;
using MusicLib.Api;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLib.Model.Music
{
    public class NetMusic
    {
        public ApiType ApiType { get; private set; }

        public string Id { get; private set; }

        public string Title { get; private set; }

        public string Author { get; private set; }

        public string Url { get; private set; }

        public string FileUrl { get; private set; }

        public string FilePath { get; private set; }

        public bool Downloaded { get; private set; } = false;

        public bool CheckDownloaded()
        {
            Downloaded = File.Exists(FilePath);
            return Downloaded;
        }

        public NetMusic(SongDetail detail, SongUrl url, ApiType api)
        {
            ApiType = api;
            Id = detail.Id;
            Title = detail.Name;
            Author = string.Join(" / ", detail.Artists);
            Url = detail.Url;
            FileUrl = url.Url;
            FilePath = GetPath();
            CheckDownloaded();
        }

        public CancellationTokenSource CancellationToken;
        public async Task Download()
        {
            if (CheckDownloaded())
            {
                return;
            }
            var downloader = new DownloadService(new DownloadConfiguration()
            {
                BufferBlockSize = 4096,
                ChunkCount = 5,
                OnTheFlyDownload = false,
                ParallelDownload = true,
            });
            CancellationToken = new CancellationTokenSource();
            _ = Task.Run(() =>
              {
                  while (!(Downloaded || downloader.IsCancelled))
                  {
                      if (CancellationToken.IsCancellationRequested)
                      {
                          downloader.CancelAsync();
                          return;
                      }
                      Thread.Sleep(10);
                  }
              });
            var stream = await downloader.DownloadFileTaskAsync(FileUrl);
            if (CancellationToken.IsCancellationRequested)
                return;
            stream.Seek(0, SeekOrigin.Begin);
            Directory.CreateDirectory(GetTempFolder());
            var fs = File.OpenWrite(FilePath);
            var buffer = new byte[4096];
            int osize;
            while ((osize = stream.Read(buffer, 0, buffer.Length)) > 0)
                fs.Write(buffer, 0, osize);
            fs.Close();
            fs.Dispose();
            stream.Close();
            stream.Dispose();
            Downloaded = true;
        }

        static string GetTempFolder()
            => Path.Combine(Utils.GetProgramPath(), "Temp");

        string GetPath() => Path.Combine(GetTempFolder(), $"{ApiType}-{Id}.musictemp");
    }
}
