using MusicLib.Model.Music;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MusicLib
{
    public static class PlayList
    {
        static int id = 0;

        public static List<PlayItem> List = new List<PlayItem>();

        public static PlayItem Downloading { get; private set; }

        public static void Add(NetMusic music, string user)
        {
            lock (List)
            {
                PlayItem item = new PlayItem(music, user, ++id);
                List.Add(item);
            }
        }

        public static void Remove(int n)
        {
            lock (List)
            {
                PlayItem target = List[n];
                if (Downloading == target)
                {
                    target.Music.CancellationToken?.Cancel();
                }
                List.RemoveAt(n);
            }
        }

        public static void Clear()
        {
            lock (List)
            {
                Downloading?.Music.CancellationToken?.Cancel();
                List.Clear();
            }
        }

        public static PlayItem[] GetPlayList() => List.ToArray();

        static PlayList()
        {
            new Thread(async () =>
            {
                while (true)
                {
                    PlayItem music = null;
                    lock (List)
                    {
                        music = List.Take(5).FirstOrDefault(x => !x.CheckDownloaded());
                    }
                    if (music != null)
                    {
                        Downloading = music;
                        int retry = 0;
                        bool success = false;
                        while (!success && retry < 6)
                        {
                            try
                            {
                                await music.Music.Download();
                                success = true;
                            }
                            catch
                            {
                                retry++;
                            }
                        }
                    }
                    Downloading = null;
                    Thread.Sleep(10);
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }

    public class PlayItem
    {
        public List<PlayItem> List => PlayList.List;

        public readonly NetMusic Music;

        public int PlayId { get; }

        public string Title => Music.Title;

        public string Author => Music.Author;

        public string User { get; }

        public PlayItem(NetMusic music, string user, int id)
        {
            Music = music;
            User = user;
            PlayId = id;
        }

        public bool CheckDownloaded()
        {
            return Music.CheckDownloaded();
        }

        private PlayableAudio _audio;
        public PlayableAudio GetPlayableAudio()
        {
            if (_audio == null && CheckDownloaded())
            {
                _audio = new PlayableAudio(Music.FilePath);
            }
            return _audio;
        }
    }
}
