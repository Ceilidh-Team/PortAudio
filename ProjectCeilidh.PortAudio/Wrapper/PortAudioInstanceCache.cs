using System;
using System.Collections.Concurrent;
using ProjectCeilidh.PortAudio.Native;

namespace ProjectCeilidh.PortAudio.Wrapper
{
    internal static class PortAudioInstanceCache
    {
        private static readonly ConcurrentDictionary<PaDeviceIndex, WeakReference<PortAudioDevice>> _deviceCache =
            new ConcurrentDictionary<PaDeviceIndex, WeakReference<PortAudioDevice>>();

        private static readonly ConcurrentDictionary<PaHostApiIndex, WeakReference<PortAudioHostApi>> _apiCache =
            new ConcurrentDictionary<PaHostApiIndex, WeakReference<PortAudioHostApi>>();
        
        public static PortAudioDevice GetPortAudioDevice(PaDeviceIndex index)
        {
            if (index.TryGetErrorCode(out var err)) throw PortAudioException.GetException(err);
            
            if (_deviceCache.TryGetValue(index, out var reference) && reference.TryGetTarget(out var target))
                return target;
            
            var device = new PortAudioDevice(index);
            _deviceCache[index] = new WeakReference<PortAudioDevice>(device);
            return device;
        }

        public static PortAudioHostApi GetHostApi(PaHostApiIndex index)
        {
            if (index.TryGetErrorCode(out var err)) throw PortAudioException.GetException(err);
            
            if (_apiCache.TryGetValue(index, out var reference) && reference.TryGetTarget(out var target))
                return target;
            
            var api = new PortAudioHostApi(index);
            _apiCache[index] = new WeakReference<PortAudioHostApi>(api);
            return api;
        }

        public static void ClearCache()
        {
            _deviceCache.Clear();
            _apiCache.Clear();
        }
    }
}