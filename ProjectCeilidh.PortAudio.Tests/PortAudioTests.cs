using System;
using Xunit;
using ProjectCeilidh.PortAudio.Native;
using ProjectCeilidh.PortAudio.Wrapper;
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
                Assert.NotNull(Pa_GetErrorText(code));
        }
    }
}
