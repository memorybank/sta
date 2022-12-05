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
    public class MobilePhone : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "MobilePhone";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Items/phone/model/props/phone4.prefab";
            _ItemProperties.ActionStatusAnimPath.Add("Assets/Items/phone/idle/PhoneAvatarPrefab.prefab");
            _ItemProperties.ActionStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Left_Arm.mask");
            _ItemProperties.SilenceStatusAnimPath.Add("Assets/Items/phone/idle/PhoneAvatarPrefab.prefab");
            _ItemProperties.SilenceStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Left_Arm.mask");
            _ItemProperties.Tags.Add("handhold");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            Dictionary<string, GameObject> d = new();
            d.Add("LeftHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "LeftHand")
                );
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                if (_IKHandLocked) return;
                _IKHandLocked = true;
                LockArmIK(slotIndex, true, true, true, true, 2);
                Debug.Log("Item Events Mobile SelfSpeaking triggered");
            });

            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, true, false, false, false, 2);
                Debug.Log("Item Events Mobile SelfInactive triggered");
            });

            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, true, false, false, false, 2);
                Debug.Log("Item Events Mobile AllInactive triggered");
            });
        }

        protected override void ExecuteExtraCmds()
        {
            _IKHandLocked = false;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, LeftHand.gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Silence, LeftHand.gameObject, 1, 0, 0));
            Debug.Log("Item Events Mobile must triggered");
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            LockArmIK(itemSlotIndex, true, false, false, true, 2);
        }
    }
}