using NAudio.Wave;
using System;
using System.Linq;

namespace MusicLib.Model.Device
{
    internal class DirectSoundOutDevice : IPlaybackDevice
    {
        public const string DeviceType = "DirectSound";

        public readonly string DeviceName;

        public readonly Guid DeviceGuid;

        public DirectSoundOutDevice(Guid guid)
        {
            DeviceGuid = guid;
            var dev = DirectSoundOut.Devices.Where(x => x.Guid == guid).FirstOrDefault();
            DeviceName = dev.Description;
        }

        public IWavePlayer CreatePlayer()
            => new DirectSoundOut(DeviceGuid);

        public string GetDeviceType() => DeviceType;

        public string GetDeviceId() => DeviceGuid.ToString();

        public string GetDeviceName() => DeviceName;

        public override string ToString()
            => $"{DeviceType} : {DeviceName}";
    }
}
