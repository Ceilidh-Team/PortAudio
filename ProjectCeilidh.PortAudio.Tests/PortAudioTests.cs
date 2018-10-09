using System;
using Xunit;
using ProjectCeilidh.PortAudio.Native;
using static ProjectCeilidh.PortAudio.Native.PortAudio;

namespace ProjectCeilidh.PortAudio.Tests
{
    public class PortAudioTests
    {
        [Fact]
        public void TestVersion()
        {
            Assert.Equal(0x130600, Pa_GetVersion());
        }

        [Fact]
        public void TestErrorText()
        {
            foreach (PaErrorCode code in Enum.GetValues(typeof(PaErrorCode)))
                Assert.NotNull(Pa_GetErrorText(code));
        }
    }
}
