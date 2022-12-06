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
    public class Microphone : BaseItem
    {

        protected override void InitProperties()
        {
            _ItemProperties.Name = "Microphone";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/SM_MicroPhone.prefab";
            _ItemProperties.Tags.Add("device");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            Dictionary<string, GameObject> d = new();
            d.Add("RightHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "RightHand")
                );
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }
        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                if (_IKHandLocked) return;
                _IKHandLocked = true;
                LockArmIK(slotIndex, false, true, false, true, 2);
                Debug.Log("Item Events microphone SelfSpeaking triggered");
            });

            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, false, false, false, true, 2);
                Debug.Log("Item Events microphone SelfInactive triggered");
            });

            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, false, false, false, true, 2);
                Debug.Log("Item Events microphone AllInactive triggered");
            });
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            LockArmIK(itemSlotIndex, false, false, false, true, 2);
        }   
    }
}