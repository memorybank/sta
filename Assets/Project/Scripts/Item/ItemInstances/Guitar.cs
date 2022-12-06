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
    public class Guitar : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Guitar";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/SM_Guitar.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_guitar_silence.wav";
            _ItemProperties.SilenceStatusAnimPath.Add("Assets/Project/Prefabs/Item_guitar_silence.prefab");
            _ItemProperties.SilenceStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Tags.Add("instrument");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            Dictionary<string, GameObject> d = new();
            d.Add("RightHand", null);
            d.Add("LeftHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "")
                );
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override Transform ItemParent()
        {
            return ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "Spine2");
        }

        protected override void RegisterAnimCallbacks()
        {
            _SilenceStatus[0]._StatusAnimations[0].Events.SetCallback("guitar_play",
                () =>
                {
                    ExitEvent.Register(_SilenceStatus[0]._StatusAnimations[0].State, () =>
                    {
                        if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && !ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                        {
                            ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Pause();
                            Debug.Log("Item Events Guitar pause triggered");
                        }
                    });

                    if (ItemEventManager.eventSequencerManager.AudioStates[ItemSlotUserDictionary[0].AvatarUser.AvatarUUID] == VoiceActivityType.Silence)
                    {
                        if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && !ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                        {
                            ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Play();
                            Debug.Log("Item Events Guitar play triggered");
                        }
                    }
                }
                );
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            // Lock right hand and poser when not speaking
            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes"+slotIndex.ToString());
                var rightHand = IKGoalDict[slotIndex]["RightHand"] == null ? IKDollNodes.Find("IKDollNodesRightHand") : IKGoalDict[slotIndex]["RightHand"].transform;                
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(rightHand, 1, 1, 2));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHandPoser, new IKTarget(rightHand, 1, 1, 2));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Guitar SelfInactive triggered");
            });

            
            // Unlock right hand and poser when speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(null, 0, 0, 2));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHandPoser, new IKTarget(null, 0, 0, 2));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Guitar SelfSpeaking triggered");
            });

            // Unlock right hand and poser when silence
            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHand, new IKTarget(null, 0, 0, 2));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightHandPoser, new IKTarget(null, 0, 0, 2));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Guitar AllInactive triggered");
            });

            ItemEventManager.AnySpeaking.AddListener(() =>
            {
                if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                {
                    ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Pause();
                    Debug.Log("Item Events Guitar pause triggered");
                }
            });
        }

        protected override void ExecuteExtraCmds()
        {
            Debug.Log("Item Events Guitar must triggered");
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            LockArmIK(itemSlotIndex, true, true, true, true, 2);
            LockArmIK(itemSlotIndex, false, true, true, true, 2);
        }
    }
}