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
    public class Like : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Like";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/SM_Thumb.prefab";
            _ItemProperties.InteractionAnimPath.Add("Assets/Project/Prefabs/Item_like_sub.prefab");
            _ItemProperties.InteractionMaskPath.Add("Assets/Project/Animations/Masks/Full_Body.mask");
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_like_sub.wav";
            _ItemProperties.Tags.Add("handhold");
            Dictionary<string, GameObject> d = new();
            d.Add("LeftHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "LeftHand")
                );
            _ItemProperties.relativeTransformNames.Add("Spine2");
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            LockArmIK(itemSlotIndex, true, false, false, true, 2);
        }
    }
}