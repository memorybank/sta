using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Playa.Audio
{
    using ItemId = UInt64;

    public enum AudioGroup
    {
        Master = 0,
        Speaker = 1,
        Effect = 2,
        Music = 3,
        UI = 4,
    }

    // TODO: make this singleton
    public class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioMixer AudioMixer;
        [SerializeField] private AudioMixerGroup MasterGroup;
        [SerializeField] private AudioMixerGroup SpeakerGroup;
        [SerializeField] private AudioMixerGroup EffectGroup;
        [SerializeField] private AudioMixerGroup MusicGroup;
        [SerializeField] private AudioMixerGroup UIGroup;

        private Dictionary<string, AudioSource> _AudioSource = new Dictionary<string, AudioSource>();
        // Indices
        private Dictionary<ItemId, List<string>> _ItemToAudios = new Dictionary<ItemId, List<string>>();

        public AudioMixerGroup GetAudioMixerGroup(AudioGroup group)
        {
            switch (group)
            {
                case AudioGroup.Master:
                    return MasterGroup;
                case AudioGroup.Speaker:
                    return SpeakerGroup;
                case AudioGroup.Effect:
                    return EffectGroup;
                case AudioGroup.Music:
                    return MusicGroup;
                case AudioGroup.UI:
                    return UIGroup;
                default:
                    return null;
            }
        }

        // Item inherent property Apis
        public void AddAudioToMixerGroup(ItemId item, string name, AudioClip clip, Audio.AudioGroup group, bool isLoop)
        {
            var audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.loop = isLoop;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = GetAudioMixerGroup(group);
            _AudioSource[GetConcatString(item,name)] = audioSource;
            if (!_ItemToAudios.ContainsKey(item))
            {
                _ItemToAudios[item] = new List<string>();
            }
            _ItemToAudios[item].Add(name);
        }

        public void ClearAllAudioForItem(ItemId item)
        {
            if (!_ItemToAudios.ContainsKey(item))
            {
                return;
            }
            foreach (var name in _ItemToAudios[item])
            {
                var audioName = GetConcatString(item, name);
                _AudioSource[audioName].Stop();
                _AudioSource.Remove(GetConcatString(item, audioName));
            }
            _ItemToAudios.Clear();
        }

        public AudioSource GetAudioSource(ItemId item, string name)
        {
            var audioName = GetConcatString(item, name);
            if (!_AudioSource.ContainsKey(audioName))
            {
                return null;
            }
            return _AudioSource[audioName];
        }

        private string GetConcatString(ItemId item, string name)
        {
            return String.Format("{0}/{1}", item.ToString(), name);
        }
    }

}
