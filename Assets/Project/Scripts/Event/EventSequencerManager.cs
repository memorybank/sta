using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Playa.Avatars;
using Playa.Common;
using Playa.Common.Utils;


namespace Playa.Event
{
    public class EventSequencerManager : MonoBehaviour
    {
        public UnityEvent _SpeakerChangeEvent; 

        //TODO:整合两个brain
        [SerializeField] private AvatarBrain[] avatarBrainList;
        private Dictionary<int, AvatarBrain> avatarBrains;

        private Timer timer;
        private Dictionary<int, VoiceActivityType> audioStates;
        private Dictionary<int, VoiceActivityType> lastStates;
        private List<int> speakerSequencer;
        private int speaker;
        private bool speakerChange = false;

        public Dictionary<int, AvatarBrain> AvatarBrains => avatarBrains;

        public Dictionary<int, VoiceActivityType> AudioStates => audioStates;

        public Dictionary<int, VoiceActivityType> LastStates  => lastStates; 

        public void VAD_Audio(VoiceActivityUnit voiceActivity, AvatarBrain audioBrain)
        {
            foreach (var kvp in avatarBrains)
            {
                if (audioBrain == kvp.Value)
                {
                    speaker = kvp.Key;
                    ChangeAudioState(speaker, voiceActivity);
                    break;
                }
            }

            if (voiceActivity.ActivityType == VoiceActivityType.Active)
            {
                AddSpeaker(speaker);
                foreach (var kvp in avatarBrains)
                {
                    if (audioStates[kvp.Key] == VoiceActivityType.Silence)
                    {
                        ChangeAudioState(kvp.Key, new VoiceActivityUnit(VoiceActivityType.Inactive, voiceActivity.DetectedTimestamp));
                    }
                }
                timer.StopTimer();
            }
            else if(voiceActivity.ActivityType == VoiceActivityType.Inactive) 
            {
                RemoveSpeaker(speaker);
                if (CheckAllNotSpeaking())
                {
                    timer.StartTimer();
                }
                else
                {
                    timer.StopTimer();
                }
            }
        }

        public int GetCurrentSpeaker()
        {
            if (speakerSequencer.Count != 0)
            {
                return speakerSequencer[0];
            }
            return -1;
        }

        private bool CheckAllNotSpeaking()
        {
            foreach (var kvp in avatarBrains)
            {
                if (audioStates[kvp.Key] != VoiceActivityType.Inactive
                    && audioStates[kvp.Key] != VoiceActivityType.Silence)
                {
                    return false;
                }
            }
            return true;
        }

        private void AddSpeaker(int speaker)
        {
            if (speakerSequencer.Count == 0)
            {
                speakerChange = true;
            }
            else
            {
                foreach (var i in speakerSequencer)
                {
                    if (i == speaker)
                    {
                        return;
                    }
                }
            }            
            speakerSequencer.Add(speaker);
        }

        private void RemoveSpeaker(int speaker)
        {
            if (speakerSequencer.Count == 0) return;
            if (speakerSequencer[0] == speaker)
            {
                speakerSequencer.RemoveAt(0);
                speakerChange = true;
                return;
            }
            int target = -1;
            for (int i = 1; i < speakerSequencer.Count; i++)
            {
                if (speakerSequencer[i] == speaker)
                {
                    target = i;
                    break;
                }
            }
            if (target != -1)
            {
                speakerSequencer.RemoveAt(target);
            }
        }

        private void ChangeAudioState(int speaker, VoiceActivityUnit voiceActivityUnit)
        {
            if (audioStates[speaker] == voiceActivityUnit.ActivityType)
            {
                Debug.Log(string.Format("VAD_Audio : {0} didn't change {1} == {2}", avatarBrains[speaker], voiceActivityUnit.ActivityType, audioStates[speaker]));
            }
            else
            {
                lastStates[speaker] = audioStates[speaker];
                audioStates[speaker] = voiceActivityUnit.ActivityType;
                avatarBrains[speaker].EventSequencer.Push(voiceActivityUnit);
                Debug.Log(string.Format("VAD_Audio : {0} change {1} => {2}", avatarBrains[speaker], lastStates[speaker], audioStates[speaker]));
            }
        }

        private string ToString (List<int> speakers)
        {
            string aa = "";
            foreach (var i in speakerSequencer)
            {
                aa = aa + i + " ";
            }
            return aa;
        }

        private void Awake()
        {
            _SpeakerChangeEvent = new UnityEvent();
        }

        void Start()
        {
            timer = new Timer();
            timer.StartTimer();
            audioStates = new Dictionary<int, VoiceActivityType>();
            lastStates = new Dictionary<int, VoiceActivityType>();
            avatarBrains = new Dictionary<int, AvatarBrain>();
            speakerSequencer = new List<int>();
            //TODO: 更多人
            for (int i = 0; i < avatarBrainList.Length; i++)
            {
                audioStates.Add(avatarBrainList[i].AvatarUser.AvatarUUID, VoiceActivityType.Inactive);
                lastStates.Add(avatarBrainList[i].AvatarUser.AvatarUUID, VoiceActivityType.Inactive);
                avatarBrains.Add(avatarBrainList[i].AvatarUser.AvatarUUID, avatarBrainList[i]);
            }
        }

        void Update()
        {
            if (timer.ElapsedTime() >= 3)
            {
                foreach (var kvp in avatarBrains)
                {
                    //todo add silence activity DetectedTimeStamp
                    ChangeAudioState(kvp.Key, new VoiceActivityUnit(VoiceActivityType.Silence));
                }
                timer.StopTimer();
            }

            if (speakerChange)
            {
                _SpeakerChangeEvent.Invoke();
                speakerChange = false;
            }
        }
    }
}

