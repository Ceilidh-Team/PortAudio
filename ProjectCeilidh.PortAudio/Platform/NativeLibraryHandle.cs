using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ProjectCeilidh.PortAudio.Platform
{
    internal abstract class NativeLibraryHandle : IDisposable
    {
        private readonly Dictionary<string, IntPtr> _symbolTable = new Dictionary<string, IntPtr>();

        public T GetSymbolDelegate<T>(string name, bool throwOnError = true) where T : Delegate
        {
            if (!_symbolTable.TryGetValue(name, out var addr)) _symbolTable[name] = addr = GetSymbolAddress(name);

            if (addr == IntPtr.Zero)
            {
                if (throwOnError) throw new EntryPointNotFoundException();
                return default;
            }

            try
            {
                return Marshal.GetDelegateForFunctionPointer<T>(addr);
            }
            catch (MarshalDirectiveException)
            {
                if (throwOnError) throw;
                return default;
            }
        }

        public unsafe ref T GetSymbolReference<T>(string name) where T : struct 
        {
            if (!_symbolTable.TryGetValue(name, out var addr)) _symbolTable[name] = addr = GetSymbolAddress(name);

            if (addr == IntPtr.Zero) throw new EntryPointNotFoundException();

            return ref Unsafe.AsRef<T>(addr.ToPointer());
        }

        protected abstract IntPtr GetSymbolAddress(string name);

        public abstract void Dispose();
    }
}
