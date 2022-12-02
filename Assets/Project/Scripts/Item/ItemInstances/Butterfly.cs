using Animancer;
using Cinemachine;
using Playa.App;
using Playa.App.Actors;
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
    public class Butterfly : FollowItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Butterfly";
            _ItemProperties.ObjectAssetPath = "Assets/Items/butterfly/model/props/butterfly.prefab";
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Follow };
        }

        protected override Quaternion ItemRotationFromUser(AvatarUser user)
        {
            return _ActorsUtils._ActorsManager.GetAvatarPosition(user).rotation;
        }

        protected override void AddClips()
        {
            _Clips.Add(0, "Assets/Items/butterfly/model/props/butterflyIdle.anim");
        }
    }
}
