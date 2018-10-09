using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ProjectCeilidh.PortAudio.Native
{
    internal class MarshalAsAsciiString : ICustomMarshaler
    {
        private static MarshalAsAsciiString Instance { get; } = new MarshalAsAsciiString();

        public void CleanUpManagedData(object managedObj) { }

        public void CleanUpNativeData(IntPtr pNativeData) { }

        public int GetNativeDataSize() => -1;

        public IntPtr MarshalManagedToNative(object managedObj) => throw new NotSupportedException();

        public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
        {
            var sb = new StringBuilder();

            for (var ptr = (byte*)pNativeData.ToPointer(); * ptr != 0; ptr++)
                sb.Append((char) *ptr);

            return sb.ToString();
        }

        public static ICustomMarshaler GetInstance(string cookie) => Instance;
    }
}
