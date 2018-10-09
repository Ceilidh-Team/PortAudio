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
            Assert.Equal(new Version(19, 6, 0), Pa_GetVersionInfo().Version);
        }

        [Fact]
        public void TestErrorText()
        {
            foreach (PaErrorCode code in Enum.GetValues(typeof(PaErrorCode)))
                Assert.NotNull(Pa_GetErrorText(code));
        }
    }
}
