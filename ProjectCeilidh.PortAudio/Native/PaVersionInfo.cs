using System;
using System.Runtime.InteropServices;

namespace ProjectCeilidh.PortAudio.Native
{
    internal readonly struct PaVersionInfo
    {
        public string VersionControlRevision => _versionControlRevision == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(_versionControlRevision);

        public string VersionText => _versionText == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(_versionText);

        public Version Version => new Version(VersionMajor, VersionMinor, VersionSubMinor);

#pragma warning disable 649 // Disable unassigned field warning
        public int VersionMajor { get; }
        public int VersionMinor { get;  }
        public int VersionSubMinor { get; }
        private readonly IntPtr _versionControlRevision;
        private readonly IntPtr _versionText;
#pragma warning restore 649 // Disable unassigned field warning
    }
}
