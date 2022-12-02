using System;
using Recognissimo.Utils;
using UnityEngine;

using Playa.Audio.MicrophoneEngine;

namespace Playa.Audio.ASR
{
    public class VoskMicrophoneSourceAdapter : Recognissimo.Components.SpeechSource
    {
        public AvatarAudioSource audioSource;

        public override int SampleRate => audioSource.SampleRate;

        public override void StartProduce()
        {
            audioSource.SamplesReady += OnSamplesReadyAdapted;
        }

        public override void StopProduce()
        {
            audioSource.SamplesReady -= OnSamplesReadyAdapted;
        }

        public void OnSamplesReadyAdapted(object sent, Playa.Audio.SamplesReadyEvent e)
        {
            Recognissimo.Components.SpeechSource.SamplesReadyEvent samplesReadyEvent = 
                new Recognissimo.Components.SpeechSource.SamplesReadyEvent(e.Samples, e.Length);
            OnSamplesReady(samplesReadyEvent);
        }
    }
}