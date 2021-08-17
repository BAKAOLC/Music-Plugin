using MusicLib;
using MusicLib.Api;
using MusicLib.Model.Device;
using MusicLib.Model.Music;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicPluginWindow
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        IPlaybackDevice[] PlaybackDevices;
        IPlaybackDevice PlaybackDevice;

        MusicApi Api = new MusicApi();

        Player Player;

        PlayableAudio Playing;

        public bool IsOpened = false;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                IsOpened = true;
                Player = new Player();
                UpdateDeviceList(true);
                WindowMusicApiLabel.DataContext = Api;
                WindowVolumeSlider.DataContext = Player;
                WindowVolumeText.DataContext = Player;
                WindowOutputPlayInfoCheckBox.DataContext = Player.PlayerConfig;
                WindowOutputPlayInfoTextBox.DataContext = Player.PlayerConfig;
                new Thread(UpdateThread)
                {
                    IsBackground = true
                }.Start();
                new Thread(UpdateThread2)
                {
                    IsBackground = true
                }.Start();
            };
            Closing += (s, e) =>
            {
                e.Cancel = true;
                IsOpened = false;
                Application.Current.Dispatcher.Invoke(Hide);
            };
            Activated += (s, e) => IsOpened = true;
        }

        void UpdateDeviceList(bool first = false)
        {
            var devices = PlaybackDevices = OutputDevice.EnumOutputDevices();
            WindowOutputDeviceList.ItemsSource = devices.Select(x => x.ToString()).ToArray();
            if (first)
            {
                int index = Array.FindIndex(devices, x
                    => x.GetDeviceType() == Player.OutputDevice.PlaybackDevice.GetDeviceType()
                    && x.GetDeviceId() == Player.OutputDevice.PlaybackDevice.GetDeviceId());
                if (index != -1)
                {
                    WindowOutputDeviceList.SelectedIndex = index;
                    PlaybackDevice = devices[index];
                }
                else
                {
                    WindowOutputDeviceList.SelectedIndex = 0;
                    PlaybackDevice = devices[0];
                }
            }
        }

        private void UpdateDeviceList(object sender, ContextMenuEventArgs e)
            => UpdateDeviceList();

        private void DeviceSelect(object sender, SelectionChangedEventArgs e)
        {
            if (WindowOutputDeviceList.SelectedIndex != -1)
            {
                PlaybackDevice = PlaybackDevices[WindowOutputDeviceList.SelectedIndex];
                if (Player != null)
                {
                    Player.SetOutputDevice(PlaybackDevice);
                    Player.SaveConfig();
                }
            }
        }

        private void RemoveSelectMusic(object sender, RoutedEventArgs e)
        {
            if (WindowPlayList.SelectedIndex != -1)
            {
                if (WindowPlayList.SelectedIndex == 0)
                    Next();
                else
                    PlayList.Remove(WindowPlayList.SelectedIndex);
                UpdateList();
            }
        }

        void Play(PlayItem item)
        {
            Playing = item.GetPlayableAudio();
            Player.InitAudio(Playing);
            Player.Play();
            Playing.PlaybackStopped += Next;
            Application.Current.Dispatcher.Invoke(() =>
            {
                WindowPlayButton.IsEnabled = false;
                WindowPauseButton.IsEnabled = true;
                WindowTotalTime.DataContext = Playing;
                WindowCurrentTime.DataContext = Playing;
                WindowPlaySlider.IsEnabled = true;
                WindowPlaySlider.DataContext = Playing;
            });
        }

        void Resume()
        {
            if (PlayList.List.Count > 0 && Playing != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WindowPlayButton.IsEnabled = false;
                    WindowPauseButton.IsEnabled = true;
                });
                Player.Play();
            }
        }

        void Pause()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WindowPlayButton.IsEnabled = true;
                WindowPauseButton.IsEnabled = false;
            });
            Player.Pause();
        }

        void Next()
        {
            if (Playing != null)
            {
                Playing.PlaybackStopped -= Next;
                Player.Pause();
                Playing = null;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WindowCurrentTime.DataContext = null;
                    WindowTotalTime.DataContext = null;
                    WindowPlaySlider.IsEnabled = false;
                    WindowPlaySlider.DataContext = null;
                });
            }
            PlayList.Remove(0);
            UpdateList();
        }

        void Clear()
        {
            if (Playing != null)
            {
                Playing.PlaybackStopped -= Next;
                Player.Pause();
                Playing = null;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    WindowCurrentTime.DataContext = null;
                    WindowTotalTime.DataContext = null;
                    WindowPlaySlider.IsEnabled = false;
                    WindowPlaySlider.DataContext = null;
                });
            }
            PlayList.Clear();
            UpdateList();
        }

        void UpdateList()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                WindowPlayList.ItemsSource = PlayList.GetPlayList();
            });
        }

        void UpdateThread()
        {
            while (true)
            {
                if (Playing == null)
                {
                    if (PlayList.List.Count > 0 && PlayList.List[0].CheckDownloaded())
                        Play(PlayList.List[0]);
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            WindowPlayButton.IsEnabled = true;
                            WindowPauseButton.IsEnabled = false;
                            WindowCurrentTime.DataContext = null;
                            WindowTotalTime.DataContext = null;
                            WindowPlaySlider.IsEnabled = false;
                            WindowPlaySlider.DataContext = null;
                        });
                    }
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        WindowPlaySlider.IsEnabled = true;
                        Playing?.UpdateProperty();
                    });
                }
                Thread.Sleep(10);
            }
        }

        void UpdateThread2()
        {
            while (true)
            {
                UpdateCurrentPlayInfo();
                Thread.Sleep(100);
            }
        }

        public void UpdateCurrentPlayInfo()
        {
            if (Player.PlayerConfig.OutputPlayerInfo)
            {
                var file = Path.Combine(Utils.GetProgramPath(), "playinfo.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                File.WriteAllText(file, string.Format(new MusicFormatProvider(), Player.PlayerConfig.PlayerOutputFormatString,
                    Math.Max(0, PlayList.List.Count - 1), PlayList.List.FirstOrDefault(), PlayList.List.Skip(1).FirstOrDefault()));
            }
        }

        private void ResumePlayer(object sender, RoutedEventArgs e)
            => Resume();

        private void PausePlayer(object sender, RoutedEventArgs e)
            => Pause();

        private void SkipPlayer(object sender, RoutedEventArgs e)
            => Next();

        private void ClearMusic(object sender, RoutedEventArgs e)
            => Clear();

        private void PlayTimeChanging(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => Pause();

        private void PlayTimeChanged(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => Resume();

        public async Task<NetMusic> InsertMusic(string keyword, string user)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return null;
            var music = await Api.GetMusic(keyword);
            PlayList.Add(music, user);
            UpdateList();
            return music;
        }

        private async void InsertMusic(object sender, RoutedEventArgs e)
        {
            var target = WindowSearchInputBox.Text;
            WindowSearchInputBox.Text = "";
            try
            {
                await InsertMusic(target, "主播");
            }
            catch
            {

            }
        }

        private async void InsertMusicKeyboard(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var target = WindowSearchInputBox.Text;
                WindowSearchInputBox.Text = "";
                try
                {
                    await InsertMusic(target, "主播");
                }
                catch
                {
                }
            }
        }
    }
}
