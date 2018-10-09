using System;
using System.Runtime.InteropServices;

namespace ProjectCeilidh.PortAudio.Native
{
    internal struct PaHostErrorInfo
    {
        public string ErrorText => _errorText == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(_errorText);

        public PaHostApiTypeId HostApiType { get; set; }
        public long ErrorCode { get; set; }
        private readonly IntPtr _errorText;
    }
}
