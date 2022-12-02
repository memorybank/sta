using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Playa.Event;
using System;

namespace Playa.Item
{
    using ItemId = UInt64;
    public class ItemEventManager : MonoBehaviour
    {
        UnityAction<VoiceActivityUnit, int> _OnVoiceActivity;
        UnityAction _SpeakerChange;
        public EventSequencerManager eventSequencerManager;
        public ServiceLocator ServiceLocator;

        public UnityEvent SelfSpeaking, SelfPunctuated, SelfInactive;
        public UnityEvent AnySpeaking, AllInactive;
        public UnityEvent SpeakerChange;

        public Dictionary<BaseItem, Dictionary<int,List<UnityAction>>> ItemEventSelfSpeakingListenerList = new Dictionary<BaseItem, Dictionary<int,List<UnityAction>>>();
        public Dictionary<BaseItem, Dictionary<int, List<UnityAction>>> ItemEventSelfPunctuatedListenerList = new Dictionary<BaseItem, Dictionary<int, List<UnityAction>>>();
        public Dictionary<BaseItem, Dictionary<int, List<UnityAction>>> ItemEventSelfInactiveListenerList = new Dictionary<BaseItem, Dictionary<int, List<UnityAction>>>();
        public Dictionary<BaseItem, List<UnityAction>> ItemEventAnySpeakingListenerList = new Dictionary<BaseItem, List<UnityAction>>();
        public Dictionary<BaseItem, List<UnityAction>> ItemEventAllInactiveListenerList = new Dictionary<BaseItem, List<UnityAction>>();
        public Dictionary<BaseItem, List<UnityAction>> ItemEventSpeakerChangeListenerList = new Dictionary<BaseItem, List<UnityAction>>();

        // Start is called before the first frame update
        void Start()
        {
            TMPro.TMP_InputField AvatarIndexText = GameObject.Find("AvatarIndexText").GetComponent<TMPro.TMP_InputField>();
            AvatarIndexText.text = "0";
        }

        public void Init()
        {
            _OnVoiceActivity = OnVoiceActivityDetect;
            _SpeakerChange = OnSpeakerChange;
            ServiceLocator.EventSequencerLocal.voiceActivityEvent.AddListener(_OnVoiceActivity);
            ServiceLocator.EventSequencerRemote.voiceActivityEvent.AddListener(_OnVoiceActivity);
            eventSequencerManager._SpeakerChangeEvent.AddListener(_SpeakerChange);
        }

        public void OnVoiceActivityDetect(VoiceActivityUnit voiceActivityUnit, int speakerUUID)
        {
            TryTriggerAllEvents(speakerUUID);
        }

        public void OnSpeakerChange()
        {
            SpeakerChange.Invoke();
        }

        public void RemoveAllListeners()
        {
            SelfSpeaking.RemoveAllListeners();
            SelfPunctuated.RemoveAllListeners();
            SelfInactive.RemoveAllListeners();
            AnySpeaking.RemoveAllListeners();
            AllInactive.RemoveAllListeners();
            SpeakerChange.RemoveAllListeners();
        }

        public void RemoveAllListenersByItem(BaseItem item)
        {
            if (ItemEventSelfSpeakingListenerList.ContainsKey(item))
            {
                foreach (var d in ItemEventSelfSpeakingListenerList[item])
                {
                    foreach (UnityAction action in d.Value)
                    {
                        SelfSpeaking.RemoveListener(action);
                    }
                }
                ItemEventSelfSpeakingListenerList.Remove(item);
            }

            if (ItemEventSelfPunctuatedListenerList.ContainsKey(item))
            {
                foreach (var d in ItemEventSelfPunctuatedListenerList[item])
                {
                    foreach (UnityAction action in d.Value)
                    {
                        SelfPunctuated.RemoveListener(action);
                    }
                }
                ItemEventSelfPunctuatedListenerList.Remove(item);
            }

            if (ItemEventSelfInactiveListenerList.ContainsKey(item))
            {
                foreach (var d in ItemEventSelfInactiveListenerList[item])
                {
                    foreach (UnityAction action in d.Value)
                    {
                        SelfInactive.RemoveListener(action);
                    }
                }
                ItemEventSelfInactiveListenerList.Remove(item);
            }

            if (ItemEventAnySpeakingListenerList.ContainsKey(item))
            {
                foreach (UnityAction action in ItemEventAnySpeakingListenerList[item])
                {
                    AnySpeaking.RemoveListener(action);
                }
                ItemEventAnySpeakingListenerList.Remove(item);
            }

            if (ItemEventAllInactiveListenerList.ContainsKey(item))
            {
                foreach (UnityAction action in ItemEventAllInactiveListenerList[item])
                {
                    AllInactive.RemoveListener(action);
                }
                ItemEventAllInactiveListenerList.Remove(item);
            }

            if (ItemEventSpeakerChangeListenerList.ContainsKey(item))
            {
                foreach (UnityAction action in ItemEventSpeakerChangeListenerList[item])
                {
                    SpeakerChange.RemoveListener(action);
                }
                ItemEventSpeakerChangeListenerList.Remove(item);
            }

        }

        public void AddItemEventSelfSpeakingListener(BaseItem item, int slot, UnityAction action)
        {
            if (!ItemEventSelfSpeakingListenerList.ContainsKey(item))
            {
                ItemEventSelfSpeakingListenerList.Add(item, new Dictionary<int, List<UnityAction>>());
            }

            if (!ItemEventSelfSpeakingListenerList[item].ContainsKey(slot))
            {
                ItemEventSelfSpeakingListenerList[item].Add(slot, new List<UnityAction>());
            }

            ItemEventSelfSpeakingListenerList[item][slot].Add(action);
            SelfSpeaking.AddListener(action);
        }

        public void AddItemEventSelfPunctuatedListener(BaseItem item, int slot, UnityAction action)
        {
            if (!ItemEventSelfPunctuatedListenerList.ContainsKey(item))
            {
                ItemEventSelfPunctuatedListenerList.Add(item, new Dictionary<int, List<UnityAction>>());
            }

            if (!ItemEventSelfPunctuatedListenerList[item].ContainsKey(slot))
            {
                ItemEventSelfPunctuatedListenerList[item].Add(slot, new List<UnityAction>());
            }

            ItemEventSelfPunctuatedListenerList[item][slot].Add(action);
            SelfPunctuated.AddListener(action);
        }

        public void AddItemEventSelfInactiveListener(BaseItem item, int slot, UnityAction action)
        {
            if (!ItemEventSelfInactiveListenerList.ContainsKey(item))
            {
                ItemEventSelfInactiveListenerList.Add(item, new Dictionary<int, List<UnityAction>>());
            }

            if (!ItemEventSelfInactiveListenerList[item].ContainsKey(slot))
            {
                ItemEventSelfInactiveListenerList[item].Add(slot, new List<UnityAction>());
            }

            ItemEventSelfInactiveListenerList[item][slot].Add(action);
            SelfInactive.AddListener(action);
        }

        public void AddItemEventAnySpeakingListener(BaseItem item, UnityAction action)
        {
            if (!ItemEventAnySpeakingListenerList.ContainsKey(item))
            {
                ItemEventAnySpeakingListenerList.Add(item, new List<UnityAction>());
            }

            ItemEventAnySpeakingListenerList[item].Add(action);
            AnySpeaking.AddListener(action);
        }

        public void AddItemEventAllInactiveListener(BaseItem item, UnityAction action)
        {
            if (!ItemEventAllInactiveListenerList.ContainsKey(item))
            {
                ItemEventAllInactiveListenerList.Add(item, new List<UnityAction>());
            }

            ItemEventAllInactiveListenerList[item].Add(action);
            AllInactive.AddListener(action);
        }

        public void AddItemEventSpeakerChangeListener(BaseItem item, UnityAction action)
        {
            if (!ItemEventSpeakerChangeListenerList.ContainsKey(item))
            {
                ItemEventSpeakerChangeListenerList.Add(item, new List<UnityAction>());
            }

            ItemEventSpeakerChangeListenerList[item].Add(action);
            SpeakerChange.AddListener(action);
        }

        public void RepopulateEvents(BaseItem item)
        {
            //todo repopulate with all speakerUUID index
            RepopulateEventWithIndex(IsSelfSpeaking, ItemEventSelfSpeakingListenerList, item);
            RepopulateEventWithIndex(IsSelfPunctuated, ItemEventSelfPunctuatedListenerList, item);
            RepopulateEventWithIndex(IsSelfInActive, ItemEventSelfInactiveListenerList, item);
            RepopulateEvent(IsAnySpeaking, ItemEventAnySpeakingListenerList, item);
            RepopulateEvent(IsAllInActive, ItemEventAllInactiveListenerList, item);
            //RepopulateEvent(IsSpeakerChange, ItemEventSpeakerChangeListenerList, item);
        }

        private void TryTriggerAllEvents(int speakerUUID)
        {
            TryTriggerEventWithIndex(IsSelfSpeaking, ItemEventSelfSpeakingListenerList, speakerUUID);
            TryTriggerEventWithIndex(IsSelfPunctuated, ItemEventSelfPunctuatedListenerList, speakerUUID);
            TryTriggerEventWithIndex(IsSelfInActive, ItemEventSelfInactiveListenerList, speakerUUID);
            TryTriggerEvent(IsAnySpeaking, AnySpeaking);
            TryTriggerEvent(IsAllInActive, AllInactive);
            //TryTriggerEvent(IsSpeakerChange, SpeakerChange);
        }

        private void RepopulateEvent(Func<bool> func, Dictionary<BaseItem, List<UnityAction>> dict, BaseItem item)
        {
            if (func())
            {
                if (dict.ContainsKey(item))
                {
                    foreach(var action in dict[item])
                    {
                        action();
                    }
                }
            }
        }

        private void RepopulateEventWithIndex(Func<int, bool> func, Dictionary<BaseItem, Dictionary<int, List<UnityAction>>> dict, BaseItem item)
        {
            if (dict.ContainsKey(item))
            {
                foreach (var d in dict[item])
                {
                    if (func(item.ItemSlotUserDictionary[d.Key].AvatarUser.AvatarUUID))
                    {
                        foreach (var action in d.Value)
                        {
                            action();
                        }
                    }
                }
            }
        }

        private void TryTriggerEvent(Func<bool> func, UnityEvent condition)
        {
            if (func()) {
                condition?.Invoke();
            }
        }

        private void TryTriggerEventWithIndex(Func<int, bool> func, Dictionary<BaseItem, Dictionary<int, List<UnityAction>>> dict, int speakerUUID)
        {
            //Debug.Log(string.Format("try trigger {0} {1} c {2}", nameof(dict), _LastState.ToString(), _CurrentState.ToString()));
            foreach(var kvp in dict)
            {
                if (func(speakerUUID))
                {
                    int slotIndex = 0;
                    foreach (var u in kvp.Key.ItemSlotUserDictionary)
                    {
                        if (u.Value.AvatarUser.AvatarUUID == speakerUUID)
                        {
                            slotIndex = u.Key;
                            break;
                        }
                    }

                    if (!kvp.Value.ContainsKey(slotIndex))
                    {
                        continue;
                    }

                    foreach (var action in kvp.Value[slotIndex])
                    {
                        action();
                    }
                }
            }
        }
        private bool IsSelfSpeaking(int index)
        {
            return eventSequencerManager.AudioStates[index] == VoiceActivityType.Active;
        }

        private bool IsSelfPunctuated(int index)
        {
            return eventSequencerManager.AudioStates[index] == VoiceActivityType.Punctuated;
        }

        private bool IsSelfInActive(int index)
        {
            return eventSequencerManager.AudioStates[index] == VoiceActivityType.Inactive;
        }
        private bool IsAnySpeaking()
        {
            bool has = false;
            foreach (var kvp in eventSequencerManager.AudioStates)
            {
                if (kvp.Value == VoiceActivityType.Active)
                {
                    has = true;
                    break;
                }
            }

            if (!has)
            {
                return false;
            }

            foreach (int k in eventSequencerManager.AudioStates.Keys)
            {
                if (!WasNotSpeaking(k))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsAllInActive()
        {
            foreach (var kvp in eventSequencerManager.AudioStates)
            {
                if (kvp.Value != VoiceActivityType.Silence)
                {
                    return false;
                }
            }
            return true;
        }

        private bool WasNotSpeaking(int index)
        {
            return eventSequencerManager.LastStates[index] == VoiceActivityType.Inactive
                || eventSequencerManager.LastStates[index] == VoiceActivityType.Silence;
        }

    }
}