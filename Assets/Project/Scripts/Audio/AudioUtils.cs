using Playa.Audio.MicrophoneEngine;
using System;
using UnityEngine;

namespace Playa.Audio
{

    public class AudioUtils
    {
        // WebRTC and XunFei only supports raw, 16-bit linear PCM audio
        private const int _BytePerSample = 2;

        public static byte[] ConvertEventToBytes(int cutoff, SamplesReadyEvent e)
        {
            var len = Math.Min(cutoff, e.Length);
            var buffer = new byte[len * _BytePerSample];
            for (int i = 0; i < len; i++)
            {
                var bytes = BitConverter.GetBytes((short)(e.Samples[i]));
                buffer[i * _BytePerSample] = bytes[0];
                buffer[i * _BytePerSample + 1] = bytes[1];
            }
            return buffer;
        }
    }

}