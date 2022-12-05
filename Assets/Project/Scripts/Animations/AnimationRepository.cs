using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cfg.gesture;

namespace Playa.Animations
{
    public abstract class AnimationRepository : MonoBehaviour
    {
        [SerializeField] protected List<ClipTransition> _AnimationClips;
        [SerializeField] private Dictionary<string, AnimationClipInfo> _AnimationClipInfos;

        private Dictionary<string, int> _AnimationIndexByName;
        private Dictionary<int, int> _AnimationIndexById;

        public List<ClipTransition> AnimationClips => _AnimationClips;
        public Dictionary<string, AnimationClipInfo> AnimationClipInfos => _AnimationClipInfos;

        public AnimationRepository() 
        {
            _AnimationClipInfos = new Dictionary<string, AnimationClipInfo>();
        }

        // Generate animation clip infos from clips
        public abstract void Init();

        public void AddAnimationClip(ClipTransition toAdd)
        {
            _AnimationClips.Add(toAdd);
        }

        public void AddAnimationClipInfo(AnimationClipInfo toAdd)
        {
            _AnimationClipInfos[toAdd.ClipName] = toAdd;
            Debug.Log(string.Format("Added animtion clip info for {0}", toAdd.ClipName));
        }

        public void ResetAnimationRepository(List<ClipTransition> clip, Dictionary<string, AnimationClipInfo> clipinfo)        {
            _AnimationClips = clip;
            _AnimationClipInfos = clipinfo;
        }

        public int GetClipIndexByName(string clipName)
        {
            if (_AnimationIndexByName.ContainsKey(clipName))
            {
                return _AnimationIndexByName[clipName];
            }
            return -1;
        }

        public int GetClipIndexById(int id)
        {
            if (_AnimationIndexById.ContainsKey(id))
            {
                return _AnimationIndexById[id];
            }
            return -1;
        }

        protected void GenerateIndices()
        {
            GenerateIndexByName();
            GenerateIndexById();
        }

        protected void GenerateIndexByName()
        {
            _AnimationIndexByName = new Dictionary<string, int>();
            for (int i = 0; i < AnimationClips.Count; i++)
            {
                _AnimationIndexByName[AnimationClips[i].Clip.name] = i;
            }
        }

        protected void GenerateIndexById()
        {
            _AnimationIndexById = new Dictionary<int, int>();
            foreach (var clipInfo in AnimationClipInfos)
            {
                _AnimationIndexById[clipInfo.Value.Id] = GetClipIndexByName(clipInfo.Key);
            }
        }

    }
}