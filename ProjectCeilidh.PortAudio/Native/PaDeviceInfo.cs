using System;
using System.Runtime.InteropServices;

namespace ProjectCeilidh.PortAudio.Native
{
    /// <summary>
    /// A structure providing information and capabilities of PortAudio devices.
    /// Devices may support input, output or both input and output.
    /// </summary>
    internal struct PaDeviceInfo
    {
        public string Name => _name == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(_name);

#pragma warning disable 649 // Disable unassigned field warning
        public int StructVersion { get; set; }
        private readonly IntPtr _name;
        public PaHostApiIndex HostApiIndex { get; set; }
        public int MaxInputChannels { get; set; }
        public int MaxOutputChannels { get; set; }
        /// <summary>Default latency values for interactive peformance.</summary>
        public PaTime DefaultLowInputLatency { get; set; }
        public PaTime DefaultLowOutputLatency { get; set; }
        /// <summary>Default latency values for robust non-interactive applications (eg. playing sound files).</summary>
        public PaTime DefaultHighInputLatency { get; set; }
        public PaTime DefaultHighOutputLatency { get; set; }
        public double DefaultSampleRate { get; set; }
#pragma warning restore 649
    }
}
