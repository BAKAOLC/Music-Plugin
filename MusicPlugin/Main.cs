using System;

namespace MusicPlugin
{
    public class Main : BilibiliDM_PluginFramework.DMPlugin
    {
        public PluginConfig PluginConfig = new PluginConfig();

        public bool IsEnable { get => PluginConfig.IsEnable; set => PluginConfig.IsEnable = value; }

        public Main()
        {
            PluginName = "Music";
            PluginDesc = "弹幕点歌";
            PluginAuth = "OLC";
            PluginCont = "ritsukage@qq.com";
            PluginVer = "v0.0.2";
            PluginConfig.Load();
            ReceivedDanmaku += async (s, e) =>
            {
                if (IsEnable)
                {
                    if (e.Danmaku.MsgType == BilibiliDM_PluginFramework.MsgTypeEnum.Comment)
                    {
                        if (e.Danmaku.CommentText.StartsWith("点歌"))
                        {
                            var target = e.Danmaku.CommentText.Substring(2).Trim();
                            if (!string.IsNullOrWhiteSpace(target))
                            {
                                try
                                {
                                    var music = await ConfigWindow.InsertMusic(target, $"{e.Danmaku.UserName}(id:{e.Danmaku.UserID})");
                                    Log($"点歌成功：{music.Title} ({music.Id} - {music.ApiType})");
                                    AddDM($"点歌成功：{music.Title} ({music.Id} - {music.ApiType})");
                                }
                                catch (Exception ex)
                                {
                                    Log($"点歌失败，" + ex.Message + Environment.NewLine + ex.StackTrace);
                                }
                            }
                        }
                    }
                }
            };
        }

        public override void Start()
        {
            base.Start();
            IsEnable = true;
            PluginConfig.Save();
            Log("开始处理直播间弹幕");
            AddDM("开始处理直播间弹幕");
        }

        public override void Stop()
        {
            base.Stop();
            IsEnable = false;
            PluginConfig.Save();
            Log("停止处理直播间弹幕");
            AddDM("停止处理直播间弹幕");
        }

        MusicPluginWindow.MainWindow ConfigWindow = null;
        public override void Admin()
        {
            base.Admin();
            if (ConfigWindow == null)
            {
                ConfigWindow = new MusicPluginWindow.MainWindow();
                ConfigWindow.Show();
            }
            else
            {
                if (ConfigWindow.IsOpened)
                    ConfigWindow.Activate();
                else
                {
                    ConfigWindow.Show();
                    ConfigWindow.Activate();
                }
            }
        }

        public override void Inited()
        {
            base.Inited();
            if (IsEnable)
                Start();
        }
    }
}
