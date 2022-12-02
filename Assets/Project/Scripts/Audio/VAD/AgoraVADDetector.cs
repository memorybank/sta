using UnityEngine;
using WebRtcVadSharp;

using Playa.Audio;
using Playa.Audio.MicrophoneEngine;
using System;
using Playa.Common;

using Playa.Audio.Event;
using agora_gaming_rtc;
using Playa.Audio.Agora;

namespace Playa.Audio.VAD
{

    public class AgoraVADDetector : VADDetector
    {
        private int _MinContinousInactive = 6;
        private int _ContinuouosInactive = 0;
        public AudioSourceType AudioSourceType = AudioSourceType.Remote;

        [SerializeField] private AgoraManager _AgoraManager;

        private void Start()
        {
            var mRtcEngine = _AgoraManager.ChatRoomManager.mRtcEngine;
            mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) =>
            {
                if (speakerNumber == 0 || speakers == null)
                {
                    return;
                }

                uint tVolume = 0;
                if (AudioSourceType == AudioSourceType.Local)
                {
                    if (speakers[0].uid != 0)
                    {
                        Debug.Log(string.Format("require local but remote callback received, dismiss it {0}", speakers));
                        return;
                    }

                    Debug.Log("Agora volume " + speakers[0].volume + "speaker 0 id " + speakers[0].uid);
                    tVolume = speakers[0].volume;
                }
                else if (AudioSourceType == AudioSourceType.Remote)
                {
                    foreach (AudioVolumeInfo speaker in speakers)
                    {
                        if (speaker.uid != 0)
                        {
                            // comment: we only has one remote speaker
                            Debug.Log("Agora volume " + speaker.volume + "speaker id " + speaker.uid);
                            tVolume = speaker.volume;
                            break;
                        }
                    }
                }

                if (tVolume > 0)
                {
                    _ContinuouosInactive = 0;
                    _VoiceActivity = VoiceActivityType.Active;
                    _AvatarBrain.EventSequencer.Push(new VoiceActivityUnit(_VoiceActivity));
                    
                }
                else if (tVolume == 0)
                {
                    _ContinuouosInactive++;
                    if (_VoiceActivity is VoiceActivityType.Active)
                    {
                        _VoiceActivity = VoiceActivityType.Punctuated;
                        _AvatarBrain.EventSequencer.Push(new VoiceActivityUnit(_VoiceActivity));
                    } else if (_VoiceActivity is VoiceActivityType.Punctuated)
                    {
                        if (_ContinuouosInactive > _MinContinousInactive)
                        {
                            _VoiceActivity = VoiceActivityType.Inactive;
                            _AvatarBrain.EventSequencer.Push(new VoiceActivityUnit(_VoiceActivity));
                        }
                    }
                }
            };
        }
    }
}