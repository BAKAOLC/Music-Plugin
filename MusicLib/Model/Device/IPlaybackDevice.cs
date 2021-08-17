using NAudio.Wave;

namespace MusicLib.Model.Device
{
    public interface IPlaybackDevice : IDevice
    {
        public IWavePlayer CreatePlayer();
    }
}
