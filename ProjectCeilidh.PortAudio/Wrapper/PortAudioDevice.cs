using System;
using ProjectCeilidh.PortAudio.Native;
using static ProjectCeilidh.PortAudio.Native.PortAudio;

namespace ProjectCeilidh.PortAudio.Wrapper
{
    public class PortAudioDevice : IDisposable
    {
        public string Name => DeviceInfo.Name;
        public int MaxInputChannels => DeviceInfo.MaxInputChannels;
        public int MaxOutputChannels => DeviceInfo.MaxOutputChannels;
        public TimeSpan DefaultHighOutputLatency => DeviceInfo.DefaultHighOutputLatency.Value;
        public TimeSpan DefaultLowOutputLatency => DeviceInfo.DefaultLowOutputLatency.Value;
        public TimeSpan DefaultHighInputLatency => DeviceInfo.DefaultHighInputLatency.Value;
        public TimeSpan DefaultLowInputLatency => DeviceInfo.DefaultLowInputLatency.Value;
        public double DefaultSampleRate => DeviceInfo.DefaultSampleRate;

        internal PaDeviceIndex DeviceIndex { get; }

        private ref PaDeviceInfo DeviceInfo => ref Pa_GetDeviceInfo(DeviceIndex);

        internal PortAudioDevice(PaDeviceIndex deviceIndex)
        {
            PortAudioLifetimeRegistry.Register(this);

            DeviceIndex = deviceIndex;
        }

        public unsafe bool SupportsFormat(PortAudioSampleFormat sampleFormat, int channels, double sampleRate, TimeSpan suggestedLatency, bool asOutput)
        {
            PaStreamParameters* inputParams = default;
            PaStreamParameters* outputParams = default;

            var param = new PaStreamParameters
            {
                DeviceIndex = DeviceIndex,
                ChannelCount = channels,
                HostApiSpecificStreamInfo = IntPtr.Zero,
                SampleFormats = sampleFormat.SampleFormat,
                SuggestedLatency = new PaTime(suggestedLatency)
            };

            if (asOutput)
                outputParams = &param;
            else
                inputParams = &param;

            return Pa_IsFormatSupported(inputParams, outputParams, sampleRate) >= PaErrorCode.NoError;
        }

        private void ReleaseUnmanagedResources()
        {
            PortAudioLifetimeRegistry.UnRegister(this);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~PortAudioDevice()
        {
            ReleaseUnmanagedResources();
        }
    }
}
