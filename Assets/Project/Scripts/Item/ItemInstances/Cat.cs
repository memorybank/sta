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
    public class Cat : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Cat";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Items/cat/model/props/cat+amy.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_cat_start.wav";
            _ItemProperties.SoundCycle = false;
            _ItemProperties.ActionStatusAnimPath.Add("Assets/Project/Prefabs/Item_cat_notspeaking.prefab");
            _ItemProperties.ActionStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_Body.mask");
            _ItemProperties.SilenceStatusAnimPath.Add("Assets/Project/Prefabs/Item_cat_notspeaking.prefab");
            _ItemProperties.SilenceStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_Body.mask");
            _ItemProperties.Tags.Add("petInArms");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override void ExecuteExtraCmds()
        {
            var audio = ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name);
            audio.Play();
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, LeftHand.gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Silence, LeftHand.gameObject, 1, 0, 0));
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            // Lock right hand and poser when not speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                if (_IKHandLocked) return;
                _IKHandLocked = true;
                LockArmIK(slotIndex, true, true, true, true, 2);
                Debug.Log("Item Events Cat SelfInactive triggered");
            });


            // Unlock right hand and poser when speaking
            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, true, false, false, false, 2); 
                Debug.Log("Item Events Cat SelfSpeaking triggered");
            });

            // Unlock right hand and poser when silence
            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _IKHandLocked = false;
                LockArmIK(slotIndex, true, false, false, false, 2);
                Debug.Log("Item Events Cat AllInactive triggered");
            });
        }


        protected override void InitialIKTargets(int avatarUserIndex, Transform IKDollNodes)
        {
            LockArmIK(avatarUserIndex, true, true, false, true, 2);
        }
    }
}