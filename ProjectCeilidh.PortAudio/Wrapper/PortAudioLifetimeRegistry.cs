using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProjectCeilidh.PortAudio.Native;

namespace ProjectCeilidh.PortAudio.Wrapper
{
    internal static class PortAudioLifetimeRegistry
    {
        private static readonly object SyncObject = new object();
        private static readonly HashSet<int> RegisteredHandles = new HashSet<int>();

        public static void Register(object target)
        {
            lock (SyncObject)
            {
                if (RegisteredHandles.Count == 0)
                {
                    var err = Native.PortAudio.Pa_Initialize();
                    if (err < PaErrorCode.NoError) throw PortAudioException.GetException(err);
                }

                RegisteredHandles.Add(RuntimeHelpers.GetHashCode(target));
            }
        }

        public static void UnRegister(object target)
        {
            lock (SyncObject)
            {
                if (!RegisteredHandles.Remove(RuntimeHelpers.GetHashCode(target))) return;

                if (RegisteredHandles.Count != 0) return;

                PortAudioInstanceCache.ClearCache();
                
                var err = Native.PortAudio.Pa_Terminate();
                if (err < PaErrorCode.NoError) throw PortAudioException.GetException(err);
            }
        }
    }
}
