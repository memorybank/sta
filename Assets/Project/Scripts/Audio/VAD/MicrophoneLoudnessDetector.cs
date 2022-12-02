using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Recognissimo.Components;
using UnityEngine.Events;

using Playa.Common;
using Playa.Audio.MicrophoneEngine;
using Playa.Audio.Event;


namespace Playa.Audio.VAD
{
    public class MicrophoneLoudnessDetector : AudioDetector
    {
        public VoiceLoudnessEvent VoiceLoudnessEvent;

        [SerializeField] private int _SampleWindow;

        [SerializeField] private float _LoudnessThreshold;

        [SerializeField] private float _LoudnessSensitivity;

        private bool _isStarting;
        private float _LastLoudness;
        private int _LoudnessBufferDeclineTimes;
        public float MinLoudness;

        public float GetLoudinessFromAudioClip()
        {
            if (!_SpeechSource.IsPlaying)
            {
                return 0;
            }

            int startPosition = _SpeechSource.GetClipPosition() - _SampleWindow;
            if (startPosition < 0)
            {
                return 0;
            }
            
            float[] waveData = new float[_SampleWindow];
            _SpeechSource.Clip.GetData(waveData, startPosition);
            float totalLoudness = 0;
            for (int i = 0; i < _SampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }

            return totalLoudness * _LoudnessSensitivity / _SampleWindow;
        }

        // Start is called before the first frame update
        void Start()
        {
            Debug.Assert(_SampleWindow > 0, "_SampleWindow必须大于0");
            Debug.Assert(_LoudnessThreshold >= 0, "_LoudinessThreshold必须大于等于0");
            Debug.Assert(_LoudnessSensitivity > 0, "_AudioSensitivity必须大于0");

            MinLoudness = _LoudnessThreshold;
        }

        // Update is called once per frame
        void Update()
        {
            float loudiness = GetLoudinessFromAudioClip();
            if (loudiness > _LastLoudness)
            {
                _LastLoudness = loudiness;
                _LoudnessBufferDeclineTimes = 0;
            }
            else
            {
                _LastLoudness -= _LoudnessSensitivity * (float)0.005 * (float)Mathf.Pow(2, (float)Mathf.Max(_LoudnessBufferDeclineTimes, 8));
                if (_LastLoudness < 0)
                {
                    _LastLoudness = 0;
                }
                _LoudnessBufferDeclineTimes++;
            }
            if (_LastLoudness > _LoudnessThreshold)
            {
                if (!_isStarting)
                {
                    _isStarting = true;
                    VoiceLoudnessEvent?.Invoke(new VoiceLoudnessUnit(_LastLoudness));
                }
            }
            else
            {
                if (_isStarting)
                {
                    _isStarting = false;
                    //VoiceLoudnessEvent?.Invoke(new VoiceLoudnessUnit(_LastLoudness));
                }
            }
        }

        private void OnSamplesReady(object sender, SamplesReadyEvent e)
        {            
        }

        public override string GetDetectedResult()
        {
            return _LastLoudness.ToString();
        }
    }
}
