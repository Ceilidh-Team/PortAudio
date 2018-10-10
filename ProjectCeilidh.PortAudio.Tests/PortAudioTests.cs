using System;
using Xunit;
using ProjectCeilidh.PortAudio.Native;
using static ProjectCeilidh.PortAudio.Native.PortAudio;

namespace ProjectCeilidh.PortAudio.Tests
{
    public class PortAudioTests
    {
        [Fact]
        public void TestInitialize()
        {
            using (PortAudioContext.EnterContext())
            {
                // Just make sure init and destroy works
            }
        }

        [Fact]
        public void TestErrorText()
        {
            foreach (PaErrorCode code in Enum.GetValues(typeof(PaErrorCode)))
                Assert.NotEqual(IntPtr.Zero, Pa_GetErrorText(code));
        }
    }
}
