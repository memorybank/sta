using Recognissimo.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Playa.Common;


namespace Playa.Audio.MicrophoneEngine
{
    public class MicrophoneSource : AvatarAudioSource
    {
        private const string GreetingMessage = "Press 'Space' or tap to start/stop microphone";
        private const string RecognitionStartedMessage = "Recording...";

        private static readonly ArrayPool<float> Pool = ArrayPool<float>.Shared;

        [SerializeField] private int _DeviceIndex;
        private int _SampleRate;
        [SerializeField] private int _MaxRecordingTime;
        [SerializeField] private int _TimeSensitivity;

        private string _DeviceName;

        private int _ClipSamplesLength;
        private int _LastPos;

        private int _MaxSamplesNum;

        private int _MinSamplesNum;

        private float _ElapsedRecordTime = 0.0f;

        // UI components
        [SerializeField] private Text text;

        override public int SampleRate => _SampleRate;

        override public void StartAudio()
        {
            _DeviceName = Microphone.devices[_DeviceIndex];
            _Clip = Microphone.Start(_DeviceName, true, _MaxRecordingTime, _SampleRate);
            _IsPlaying = true;
            _LastPos = 0;
            _ClipSamplesLength = _Clip.samples;

            _MinSamplesNum = _SampleRate / 1000 * _TimeSensitivity;

            _MaxSamplesNum = Math.Min(ArrayPool<float>.MaxArraySize, _ClipSamplesLength);
            Debug.Log(String.Format("MinSample {0}", _MinSamplesNum));
            Debug.Log(String.Format("MaxSample {0}", _MaxSamplesNum));

            text.text = RecognitionStartedMessage;
        }

        override public void StopAudio()
        {
            Microphone.End(_DeviceName);
            _IsPlaying = false;

            text.text = GreetingMessage;
        }

        override public int GetClipPosition()
        {
            if (!_IsPlaying)
            {
                return 0;
            }

            return Microphone.GetPosition(_DeviceName);
        }

        override public IEnumerator WaitForClipReady()
        {
            while (!_Clip)
            {
                yield return new WaitForSeconds(ThreadsConstants.WaitForReady);
            }
            yield return null;
        }

        void Awake()
        {
            _SampleRate = AudioSettings.outputSampleRate;
        }

        // Start is called before the first frame update
        void Start()
        {
            text.text = GreetingMessage;
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
            CheckSampleEvent();
        }

        public float GetNextElapsedRecordTime()
        {
            return _ElapsedRecordTime;
        }

        private void CheckSampleEvent()
        {
            if (!_IsPlaying)
            {
                return;
            }

            int _pos = Microphone.GetPosition(_DeviceName);

            var availableSamples = (_pos - _LastPos + _ClipSamplesLength) % _ClipSamplesLength;

            while (availableSamples > _MinSamplesNum)
            {
                var samplesLength = _MinSamplesNum;

                var samples = Pool.Rent(samplesLength);

                if (!_Clip.GetData(samples, _LastPos))
                {
                    Debug.LogError("Cannot access microphone data. Make sure you are not using the microphone elsewhere in your project");
                }

                for (var i = 0; i < samplesLength; i++)
                {
                    // to mono 16-bit PCM
                    samples[i] = (short)(short.MaxValue * samples[i]);
                }

                _ElapsedRecordTime += _TimeSensitivity / 1000.0f;

                SamplesReady?.Invoke(this, new SamplesReadyEvent(samples, samplesLength, _ElapsedRecordTime));
                Pool.Return(samples);

                availableSamples -= samplesLength;
                _LastPos = (_LastPos + samplesLength) % _ClipSamplesLength;
            }
        }
    }
}
