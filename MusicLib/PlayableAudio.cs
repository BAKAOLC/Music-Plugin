using NAudio.Wave;
using System;
using System.ComponentModel;

namespace MusicLib
{
    public class PlayableAudio : IDisposable, INotifyPropertyChanged
    {
        public PlayableAudio(string path)
        {
            WaveStream = new AudioFileReader(path);
        }

        public readonly AudioFileReader WaveStream;

        public string Path => WaveStream.FileName;

        public WaveFormat WaveFormat => WaveStream.WaveFormat;

        public float Volume { get => WaveStream.Volume; set => WaveStream.Volume = value; }

        public TimeSpan CurrentTime => WaveStream.CurrentTime;
        public double CurrentTimeSeconds
        {
            get => CurrentTime.TotalSeconds;
            set => SetCurrentTime(TimeSpan.FromSeconds(value));
        }
        public string CurrentTimeString => $"{Math.Floor(CurrentTime.TotalMinutes):00}:{CurrentTime.Seconds:00}";

        public TimeSpan TotalTime => WaveStream.TotalTime;
        public double TotalTimeSeconds => TotalTime.TotalSeconds;
        public string TotalTimeString => $"{Math.Floor(TotalTime.TotalMinutes):00}:{TotalTime.Seconds:00}";

        public TimeSpan RemainingTime => TotalTime - CurrentTime;
        public double RemainingTimeSeconds => RemainingTime.TotalSeconds;
        public string RemainingTimeString => $"{Math.Floor(RemainingTime.TotalMinutes):00}:{RemainingTime.Seconds:00}";

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProperty()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentTime"));
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentTimeSeconds"));
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentTimeString"));
                PropertyChanged(this, new PropertyChangedEventArgs("TotalTime"));
                PropertyChanged(this, new PropertyChangedEventArgs("TotalTimeSeconds"));
                PropertyChanged(this, new PropertyChangedEventArgs("TotalTimeString"));
                PropertyChanged(this, new PropertyChangedEventArgs("RemainingTime"));
                PropertyChanged(this, new PropertyChangedEventArgs("RemainingTimeSeconds"));
                PropertyChanged(this, new PropertyChangedEventArgs("RemainingTimeString"));
            }
        }

        public event Action PlaybackStopped;

        public void InitToDevice(OutputDevice device)
        {
            device.WavePlayer.Init(WaveStream);
            device.WavePlayer.PlaybackStopped += (s, e) => PlaybackStopped?.Invoke();
        }

        public void InitToPlayer(IWavePlayer player)
        {
            player.Init(WaveStream);
            player.PlaybackStopped += (s, e) => PlaybackStopped?.Invoke();
        }

        public void SetCurrentTime(TimeSpan time)
            => WaveStream.CurrentTime = time;

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    WaveStream?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
