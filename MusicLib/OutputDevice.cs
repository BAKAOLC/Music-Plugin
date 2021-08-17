using MusicLib.Model.Device;
using NAudio.Wave;
using System.Collections.Generic;
using System.Linq;

namespace MusicLib
{
    public class OutputDevice
    {
        public OutputDevice()
            => SetOutputDevice(GetDefaultOutputDevice());

        public OutputDevice(IPlaybackDevice device)
            => SetOutputDevice(device);

        public IPlaybackDevice PlaybackDevice { get; private set; }

        public IWavePlayer WavePlayer { get; private set; }

        public PlaybackState PlaybackState => WavePlayer.PlaybackState;

        public void Play() => WavePlayer?.Play();

        public void Pause() => WavePlayer?.Pause();

        public void Stop() => WavePlayer?.Stop();

        public void SetOutputDevice(IPlaybackDevice device)
        {
            PlaybackDevice = device;
            RefreshPlayer();
        }

        public void RefreshPlayer()
        {
            WavePlayer?.Dispose();
            WavePlayer = PlaybackDevice.CreatePlayer();
        }

        public static IPlaybackDevice GetDefaultOutputDevice()
            => new WaveOutDevice(-1);

        public static IPlaybackDevice[] EnumOutputDevices()
        {
            var devices = new List<IPlaybackDevice>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
                devices.Add(new WaveOutDevice(n));
            foreach (var dev in DirectSoundOut.Devices)
                devices.Add(new DirectSoundOutDevice(dev.Guid));
            return devices.ToArray();
        }

        public static IPlaybackDevice FindOutputDevice(string type, string name)
        {
            var devices = EnumOutputDevices();
            switch (type.ToLower())
            {
                case "waveout":
                    if (int.TryParse(name, out int id))
                        return devices.Where(x => x is WaveOutDevice
                        && (x as WaveOutDevice).DeviceNumber == id).FirstOrDefault();
                    break;
                case "directsound":
                    return devices.Where(x => x is DirectSoundOutDevice
                    && (x as DirectSoundOutDevice).DeviceGuid.ToString() == name).FirstOrDefault();
            }
            return null;
        }
    }
}
