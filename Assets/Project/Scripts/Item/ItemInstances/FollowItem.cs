using Animancer;
using Cinemachine;
using Playa.App;
using Playa.App.Actors;
using Playa.App.Cinemachine;
using Playa.Avatars;
using Playa.Common;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Playa.Item
{
    public class FollowItem : BaseItem
    {
        protected FollowItemController _ItemController;
        protected Dictionary<float, string> _Clips;

        protected virtual void AddClips() { }
        protected override void ExecuteExtraCmds()
        {
            _Objects[_ItemProperties.Name].transform.parent = AffectAvatarUser.GetAvatarPosition();
            _ItemController = _Objects[_ItemProperties.Name].AddComponent<FollowItemController>();
            _ItemController.Init();
            _Clips = new Dictionary<float, string>();
            AddClips();
            foreach (var kvp in _Clips)
            {
                AnimationClip clip = Addressables.LoadAssetAsync<AnimationClip>(kvp.Value).WaitForCompletion();
                _ItemController.AddClip(clip, kvp.Key);
            }
        }

        protected override void OnAvatarSpeedChange(bool isSelfChange, float speed)
        {
            _ItemController.PlayClip(speed);
        }
    }
}
