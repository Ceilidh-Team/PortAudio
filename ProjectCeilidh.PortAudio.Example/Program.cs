using System;
using ProjectCeilidh.PortAudio.Wrapper;

namespace ProjectCeilidh.PortAudio.Example
{

    public class Program
    {
        private static readonly PortAudioSampleFormat[] SampleFormats =
        {
            new PortAudioSampleFormat(PortAudioSampleFormat.PortAudioNumberFormat.Unsigned, 1),
            new PortAudioSampleFormat(PortAudioSampleFormat.PortAudioNumberFormat.Signed, 1),
            new PortAudioSampleFormat(PortAudioSampleFormat.PortAudioNumberFormat.Signed, 2),
            new PortAudioSampleFormat(PortAudioSampleFormat.PortAudioNumberFormat.Signed, 3),
            new PortAudioSampleFormat(PortAudioSampleFormat.PortAudioNumberFormat.Signed, 4),
            new PortAudioSampleFormat(PortAudioSampleFormat.PortAudioNumberFormat.FloatingPoint, 4),
        };

        public static void Main(string[] args)
        {
            foreach (var api in PortAudioHostApi.SupportedHostApis)
            {
                using (api)
                {
                    Console.WriteLine($"{api.Name}:");
                    foreach (var device in api.Devices)
                        using (device)
                        {
                            if (device.MaxOutputChannels <= 0) continue;

                            Console.WriteLine($"\t{device.Name}:");
                            foreach (var format in SampleFormats)
                                Console.WriteLine($"\t\tSupports {format}, {device.MaxOutputChannels}ch, {device.DefaultSampleRate/1000}KHz? {(device.SupportsFormat(format, 2, device.DefaultSampleRate, device.DefaultLowOutputLatency, true) ? "yes" : "no")}");
                        }
                }
            }
        }
    }
}
