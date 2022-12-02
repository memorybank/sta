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
    public class Knife : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Knife";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Items/knife/model/props/knife_yougujia.prefab";
            _ItemProperties.InteractionAnimPath.Add("Assets/Items/knife/idle/KnifeIdle.prefab");
            _ItemProperties.InteractionMaskPath.Add("Assets/Models/Full_Body.mask");
            _ItemProperties.ActionStatusAnimPath.Add("Assets/Items/knife/idle/KnifeIdle.prefab");
            _ItemProperties.ActionStatusMaskPath.Add("Assets/Models/Full_Body.mask");
            _ItemProperties.SilenceStatusAnimPath.Add("Assets/Items/knife/idle/KnifeIdle.prefab");
            _ItemProperties.SilenceStatusMaskPath.Add("Assets/Models/Full_Body.mask");
            _ItemProperties.SubStatusAnimPath.Add("Assets/Items/knife/idle/Stabbing.prefab");
            _ItemProperties.SubStatusMaskPath.Add("Assets/Models/Full_Body.mask");
            _ItemProperties.Tags.Add("weapon");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            Dictionary<string, GameObject> d = new();
            d.Add("RightHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "RightHand")
                );
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            LockArmIK(itemSlotIndex, false, false, false, true, 2);
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                if (_IKHandLocked) return;
                _IKHandLocked = true;
                LockArmIK(slotIndex, false, true, true, true, 2);
                Debug.Log("Item Events Knife SelfSpeaking triggered");
            });

            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, false, false, false, false, 2);
                Debug.Log("Item Events Knife SelfInactive triggered");
            });

            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, false, false, false, false, 2);
                Debug.Log("Item Events Knife AllInactive triggered");
            });
        }

        protected override void ExecuteExtraCmds()
        {
            _IKHandLocked = false;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, LeftHand.gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Silence, LeftHand.gameObject, 1, 0, 0));
            Debug.Log("Item Events Knife must triggered");
        }
    }
}
