using NAudio.Wave;

namespace MusicLib.Model.Device
{
    internal class WaveOutDevice : IPlaybackDevice
    {
        public const string DeviceType = "WaveOut";

        public readonly string DeviceName;

        public readonly int DeviceNumber;

        public WaveOutDevice(int n)
        {
            DeviceNumber = n;
            var caps = WaveOut.GetCapabilities(n);
            DeviceName = caps.ProductName;
        }

        public IWavePlayer CreatePlayer()
            => new WaveOutEvent()
            {
                DeviceNumber = DeviceNumber
            };

        public string GetDeviceType() => DeviceType;

        public string GetDeviceId() => DeviceNumber.ToString();

        public string GetDeviceName() => DeviceName;

        public override string ToString()
            => $"{DeviceType} : {DeviceName}";
    }
}
