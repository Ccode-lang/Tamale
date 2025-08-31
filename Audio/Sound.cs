using NAudio.Wave;
using Silk.NET.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Tamale.Audio
{
    // Represents a sound loaded from a WAV file
    public class Sound
    {
        public uint buffer;

        public unsafe Sound(string file) {
            WaveFileReader reader = new WaveFileReader(file);

            byte[] data = new byte[reader.Length];
            reader.Read(data, 0, data.Length);

            var format = (reader.WaveFormat.BitsPerSample, reader.WaveFormat.Channels) switch
            {
                (16, 1) => BufferFormat.Mono16,
                (16, 2) => BufferFormat.Stereo16,
                (8, 1) => BufferFormat.Mono8,
                (8, 2) => BufferFormat.Stereo8,
                _ => throw new NotSupportedException("Unsupported WAV format")
            };

            buffer = AudioVars.al.GenBuffer();

            fixed (byte* p = data)
                AudioVars.al.BufferData(buffer, format, p, data.Length, reader.WaveFormat.SampleRate);
        }

        // Play the sound using the default source
        public void Play() {
            AudioVars.al.SetSourceProperty(AudioVars.source, SourceInteger.Buffer, buffer);
            AudioVars.al.SourcePlay(AudioVars.source);
        }
    }
}
