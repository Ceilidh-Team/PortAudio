using System;
using System.Collections.Generic;
using ProjectCeilidh.PortAudio.Native;
using static ProjectCeilidh.PortAudio.Native.PortAudio;

namespace ProjectCeilidh.PortAudio.Wrapper
{
    public class PortAudioHostApi : IDisposable
    {
        public static IEnumerable<PortAudioHostApi> SupportedHostApis
        {
            get
            {
                using (PortAudioContext.EnterContext())
                {
                    var count = Pa_GetHostApiCount();

                    for (PaHostApiIndex i = default; i < count; i++)
                        yield return new PortAudioHostApi(i);
                }
            }
        }

        public string Name => ApiInfo.Name;

        public IEnumerable<PortAudioDevice> Devices
        {
            get
            {
                for (var i = 0; i < ApiInfo.DeviceCount; i++)
                {
                    var deviceIndex = Pa_HostApiDeviceIndexToDeviceIndex(_apiIndex, i);
                    if (deviceIndex.TryGetErrorCode(out var err)) throw PortAudioException.GetException(err);
                    yield return new PortAudioDevice(deviceIndex);
                }
            }
        }

        public PortAudioDevice DefaultOutputDevice => new PortAudioDevice(ApiInfo.DefaultOutputDevice);
        public PortAudioDevice DefaultInputDevice => new PortAudioDevice(ApiInfo.DefaultInputDevice);

        private ref PaHostApiInfo ApiInfo => ref Pa_GetHostApiInfo(_apiIndex);
        private readonly PaHostApiIndex _apiIndex;

        internal PortAudioHostApi(PaHostApiIndex index)
        {
            PortAudioLifetimeRegistry.Register(this);

            _apiIndex = index;
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

        ~PortAudioHostApi()
        {
            ReleaseUnmanagedResources();
        }
    }
}
