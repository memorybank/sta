using UnityEngine;
using WebRtcVadSharp;

using Playa.Audio;
using Playa.Audio.MicrophoneEngine;
using System;
using Playa.Common;

using Playa.Audio.Event;

namespace Playa.Audio.VAD
{

    public class WebRTCVADDetector : VADDetector
    {
        // sentence breakpoint 40*5=200ms
        private const int _MinContinousInactive = 60;
        private const int _MinContinousPunctuated = 10;
        private const int _MinContinousActive = 3;

        private WebRtcVad _WebRTCVAD;
        private int _FrameSize;

        private int _ContinuouosActive;
        private int _ContinuouosInactive;

        [SerializeField] private string _Name;        

        // TODO: Refactor this, lipsync should not be part of vad detector
        public OVRLipSyncContext ovrLipSyncContext;

        public bool isSampling;
        // Start is called before the first frame update
        private void StartRecordingSample()
        {
            _VoiceActivity = VoiceActivityType.Inactive;
            SampleRate rate;
            switch (_SpeechSource.SampleRate)
            {
                case 8000:
                    {
                        rate = SampleRate.Is8kHz;
                        break;
                    }
                case 16000:
                    {
                        rate = SampleRate.Is16kHz;
                        break;
                    }
                case 32000:
                    {
                        rate = SampleRate.Is32kHz;
                        break;
                    }
                case 48000:
                    {
                        rate = SampleRate.Is48kHz;
                        break;
                    }
                case 44100:
                    {
                        throw new Exception("Sample rate is 44100");
                    }
                default:
                    {
                        throw new Exception("Sample rate not found");
                    }
            }
            Debug.Log(string.Format("VAD sample rate {0} {1}", _Name, rate));
            _WebRTCVAD = new WebRtcVad()
            {
                OperatingMode = OperatingMode.VeryAggressive,
                FrameLength = FrameLength.Is30ms,
                SampleRate = rate
            };
            _FrameSize = (int)_WebRTCVAD.SampleRate / 1000 * (int)_WebRTCVAD.FrameLength;
            _SpeechSource.SamplesReady += OnSamplesReady;
        }

        private void Awake()
        {
        }

        // Update is called once per frame
        void Update()
        {
            var spaceReleased = Input.GetKeyUp(KeyCode.Space);
            var touchEnded = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;

            if (!spaceReleased && !touchEnded)
            {
                return;
            }

            if (isSampling)
            {
                _SpeechSource.SamplesReady = null;
                isSampling = false;
            }
            else
            {
                StartRecordingSample();
            }
        }

        public void OnSamplesReady(object sender, SamplesReadyEvent e)
        {
            var buffer = AudioUtils.ConvertEventToBytes(_FrameSize, e);
            Debug.Log(string.Format("VAD webrtc {0} buffer length {1}", _Name, buffer.Length));
            
            var hasSpeech = _WebRTCVAD.HasSpeech(buffer);
            if (!hasSpeech)
            {
                float[] emptySample = new float[e.Samples.Length];
                ovrLipSyncContext.ProcessAudioSamples(emptySample, 0);
                _ContinuouosActive = 0;
                _ContinuouosInactive++;
            }
            else
            {
                ovrLipSyncContext.ProcessAudioSamples(e.Samples, 0);
                _ContinuouosActive++;
                _ContinuouosInactive = 0;
            }

            Debug.Log(String.Format("VAD {0} has speech {1}", _Name, hasSpeech));

            if ((_VoiceActivity is VoiceActivityType.Inactive || _VoiceActivity is VoiceActivityType.Punctuated)
                && _ContinuouosActive >= _MinContinousActive)
            {
                _VoiceActivity = VoiceActivityType.Active;
                _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(_VoiceActivity, e.StartTimestamp), _AvatarBrain);
            }

            if (_VoiceActivity is VoiceActivityType.Active && _ContinuouosInactive >= _MinContinousPunctuated)
            {
                _VoiceActivity = VoiceActivityType.Punctuated;
                _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(_VoiceActivity, e.StartTimestamp), _AvatarBrain);
            }

            if (_VoiceActivity is VoiceActivityType.Punctuated && _ContinuouosInactive >= _MinContinousInactive)
            {
                _VoiceActivity = VoiceActivityType.Inactive;
                _EventSequencerManager.VAD_Audio(new VoiceActivityUnit(_VoiceActivity, e.StartTimestamp), _AvatarBrain);
            }
        }
    }
}