using System;
using System.IO;
using System.Runtime.InteropServices;
using ProjectCeilidh.PortAudio.Native;
using static ProjectCeilidh.PortAudio.Native.PortAudio;

namespace ProjectCeilidh.PortAudio.Wrapper
{
    /// <summary>
    /// A PortAudio device stream driven by blocking read/write calls, rather than callbacks.
    /// </summary>
    /// <inheritdoc />
    public unsafe class PortAudioDeviceStream : Stream
    {
        private static readonly PaStreamFinishedCallback StreamFinishedCallback = OnStreamFinished;

        public PortAudioSampleFormat SampleFormat { get; }
        public int Channels { get; }
        public double SampleRate { get; }
        public TimeSpan SuggestedLatency { get; }

        public override bool CanRead { get; }
        public override bool CanSeek => false;
        public override bool CanWrite { get; }
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        private GCHandle _handle;
        private readonly PaStream _stream;

        public PortAudioDeviceStream(PortAudioDevice device, bool asOutput, int channelCount, PortAudioSampleFormat sampleFormat, TimeSpan suggestedLatency, double sampleRate)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            PortAudioLifetimeRegistry.Register(this);

            SampleFormat = sampleFormat;
            Channels = channelCount;
            SampleRate = sampleRate;
            SuggestedLatency = suggestedLatency;

            PaStreamParameters* inputParams = default;
            PaStreamParameters* outputParams = default;

            var param = new PaStreamParameters
            {
                DeviceIndex = device.DeviceIndex,
                ChannelCount = channelCount,
                HostApiSpecificStreamInfo = IntPtr.Zero,
                SampleFormats = sampleFormat.SampleFormat,
                SuggestedLatency = new PaTime(suggestedLatency)
            };

            if (asOutput)
                outputParams = &param;
            else
                inputParams = &param;

            CanWrite = asOutput;
            CanRead = !asOutput;

            _handle = GCHandle.Alloc(this);

            var err = Pa_OpenStream(out _stream, inputParams, outputParams, sampleRate, 512, PaStreamFlags.NoFlag, null, GCHandle.ToIntPtr(_handle));
            if (err < PaErrorCode.NoError) throw PortAudioException.GetException(err);

            err = Pa_SetStreamFinishedCallback(_stream, StreamFinishedCallback);
            if (err < PaErrorCode.NoError) throw PortAudioException.GetException(err);

            err = Pa_StartStream(_stream);
            if (err < PaErrorCode.NoError) throw PortAudioException.GetException(err);
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead) throw new NotSupportedException();

            fixed (byte* ptr = &buffer[offset])
            {
                var err = Pa_ReadStream(_stream, new IntPtr(ptr), (ulong) (count / Channels / SampleFormat.FormatSize));
                if (err == PaErrorCode.NoError)
                    return count / Channels / SampleFormat.FormatSize;

                throw PortAudioException.GetException(err);
            }
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite) throw new NotSupportedException();

            fixed (byte* ptr = &buffer[offset])
            {
                var err = Pa_WriteStream(_stream, new IntPtr(ptr), (ulong) (count / Channels / SampleFormat.FormatSize));
                if (err != PaErrorCode.NoError) throw PortAudioException.GetException(err);
            }
        }

        /// <summary>
        /// Invoked when the stream is finished.
        /// </summary>
        public event StreamFinishedEventHandler StreamFinished;

        private void ReleaseUnmanagedResources()
        {
            var err = Pa_CloseStream(_stream);

            PortAudioLifetimeRegistry.UnRegister(this);

            if (err < PaErrorCode.NoError) throw PortAudioException.GetException(err);
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing && _handle.IsAllocated)
                _handle.Free();
            GC.SuppressFinalize(this);
        }

        ~PortAudioDeviceStream()
        {
            ReleaseUnmanagedResources();
        }

        private static void OnStreamFinished(IntPtr userData)
        {
            var handle = GCHandle.FromIntPtr(userData);

            if (!handle.IsAllocated || !(handle.Target is PortAudioDeviceStream stream)) return;

            handle.Free();
            stream.StreamFinished?.Invoke(stream, EventArgs.Empty);
        }
    }
}
