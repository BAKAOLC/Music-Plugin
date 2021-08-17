using MusicLib.Model.Config;
using MusicLib.Model.Device;
using NAudio.Wave;
using System;
using System.ComponentModel;

namespace MusicLib
{
    public class Player : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DeviceConfig DeviceConfig { get; private set; }

        public OutputDevice OutputDevice { get; private set; }

        public PlayerConfig PlayerConfig { get; private set; }

        public PlayableAudio CurrentAudio { get; private set; }

        public PlaybackState PlaybackState => OutputDevice.PlaybackState;

        public bool Playing => PlaybackState == PlaybackState.Playing;

        public TimeSpan CurrentTime => CurrentAudio == null ? TimeSpan.Zero : CurrentAudio.CurrentTime;

        public TimeSpan TotalTime => CurrentAudio == null ? TimeSpan.Zero : CurrentAudio.TotalTime;

        int _volume = 100;
        public int Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                PlayerConfig.Volume = _volume;
                if (CurrentAudio != null)
                    CurrentAudio.Volume = _volume / 100f;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Volume"));
            }
        }

        public Player()
        {
            LoadConfig();
            IPlaybackDevice device = OutputDevice.FindOutputDevice(DeviceConfig.Type, DeviceConfig.Id);
            if (device == null)
                device = OutputDevice.GetDefaultOutputDevice();
            DeviceConfig.Type = device.GetDeviceType();
            DeviceConfig.Id = device.GetDeviceId();
            OutputDevice = new OutputDevice(device);
        }

        public Player(IPlaybackDevice device)
        {
            LoadConfig();
            DeviceConfig.Type = device.GetDeviceType();
            DeviceConfig.Id = device.GetDeviceId();
            OutputDevice = new OutputDevice(device);
        }

        public void LoadConfig()
        {
            DeviceConfig = new DeviceConfig();
            DeviceConfig.Load();
            IPlaybackDevice device = OutputDevice.FindOutputDevice(DeviceConfig.Type, DeviceConfig.Id);
            if (device == null)
                device = OutputDevice.GetDefaultOutputDevice();
            OutputDevice?.SetOutputDevice(device);
            DeviceConfig.Type = device.GetDeviceType();
            DeviceConfig.Id = device.GetDeviceId();
            PlayerConfig = new PlayerConfig();
            PlayerConfig.Load();
            Volume = PlayerConfig.Volume;
        }

        public void SaveConfig()
        {
            DeviceConfig.Save();
            PlayerConfig.Volume = Volume;
            PlayerConfig.Save();
        }

        public void InitAudio(PlayableAudio audio)
        {
            if (CurrentAudio != null)
            {
                OutputDevice?.Stop();
                OutputDevice?.RefreshPlayer();
            }
            audio.Volume = _volume / 100f;
            audio.InitToDevice(OutputDevice);

            CurrentAudio = audio;
        }

        public void SetOutputDevice(IPlaybackDevice device)
        {
            OutputDevice.SetOutputDevice(device);
            DeviceConfig.Type = device.GetDeviceType();
            DeviceConfig.Id = device.GetDeviceId();
        }

        public void Play()
        {
            if (CurrentAudio != null)
                OutputDevice?.Play();
        }

        public void Pause()
        {
            if (CurrentAudio != null)
                OutputDevice?.Pause();
        }

        public void Stop()
        {
            if (CurrentAudio != null)
                OutputDevice?.Stop();
        }

        public void SetCurrentTime(TimeSpan time)
            => CurrentAudio?.SetCurrentTime(time);
    }
}
