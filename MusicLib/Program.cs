using MusicLib.Api;
using MusicLib.Model.Config;
using MusicLib.Model.Device;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading;

namespace MusicLib
{
    class Program
    {
        static IPlaybackDevice PlaybackDevice;

        static Player Player;

        static void Main(string[] args)
        {
            Console.Clear();
            ChoosePlayDevice();
            Console.Clear();
            Player = new Player(PlaybackDevice);
            Player.SaveConfig();
            while (true)
            {
                Console.WriteLine("请输入想要播放的曲目：");
                var keyword = Console.ReadLine();
                Console.WriteLine("正在搜索中……");
                var music = new MusicApi().GetMusic(keyword).Result;
                Console.WriteLine("搜索结果：");
                Console.WriteLine($"{music.Id} |  {music.Title}  作者：{music.Author}");
                Console.WriteLine("开始下载文件");
                music.Download().Wait();
                Console.WriteLine("下载完成");
                PlayMusic(music.FilePath, music.Title);
            }
        }

        static void ChoosePlayDevice()
        {
            var devices = OutputDevice.EnumOutputDevices();
            int n = 0;
            foreach (var device in devices)
                Console.WriteLine($"{n++}: {device}");
            var deviceConfig = new DeviceConfig();
            deviceConfig.Load();
            var lastDevice = OutputDevice.FindOutputDevice(deviceConfig.Type, deviceConfig.Id);
            if (lastDevice != null)
                Console.WriteLine($"-1: 上次使用的设备({lastDevice.GetDeviceType()} : {lastDevice.GetDeviceName()})");
            Console.WriteLine("=======================");
            int choose = -2;
            bool success = false;
            while (!success)
            {
                Console.WriteLine("请选择播放设备：");
                success = int.TryParse(Console.ReadLine(), out choose);
                success = success && ((lastDevice != null && choose >= -1) || (choose >= 0)) && choose < devices.Length;
                if (!success)
                    Console.WriteLine("输入错误");
            }
            PlaybackDevice = choose == -1 ? lastDevice : devices[choose];
            Console.WriteLine("播放设备已选择：" + PlaybackDevice.ToString());
        }

        static void PlayMusic(string file, string name)
        {
            Console.WriteLine("开始读取音频文件：" + Path.GetFullPath(file));
            var audio = new PlayableAudio(file);
            Console.WriteLine("装载音频文件");
            Player.InitAudio(audio);
            Console.WriteLine("开始播放");
            Player.Play();
            void UpdateState()
            {
                Console.Title = $"{PlaybackDevice} | {name} | {TimeSpanToString(Player.CurrentTime)} : {TimeSpanToString(Player.TotalTime)} | Volume: {Player.Volume:N0}% | {Player.PlaybackState}";
                Console.CursorLeft = 0;
                Console.Write($"{TimeSpanToString(Player.CurrentTime)} : {TimeSpanToString(Player.TotalTime)}");
            };
            while (Player.PlaybackState != PlaybackState.Stopped)
            {
                Thread.Sleep(10);
                UpdateState();
            }
            Console.WriteLine();
        }

        static string TimeSpanToString(TimeSpan time)
            => $"{Math.Floor(time.TotalHours):00}:{time.Minutes:00}:{time.Seconds:00}:{time.Milliseconds:000}";
    }
}
