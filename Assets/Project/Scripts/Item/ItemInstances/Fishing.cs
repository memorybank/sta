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
    public class Fishing : BaseItem
    {
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Fishing";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Items/fishing/model/props/fishing pole_static 1.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_fishing_sub.wav";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_fishing_actionidle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.SecondIdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Right_Arm.mask");
            _ItemProperties.SubStatusAnimPath.Add("Assets/Project/Prefabs/Item_fishing_sub.prefab");
            _ItemProperties.SubStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Tags.Add("hobby");
            Dictionary<string, GameObject> d = new();
            d.Add("RightHand", null);
            d.Add("LeftHand", null);
            d.Add("LeftElbow", null);
            d.Add("RightElbow", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "LeftHand")
                );
            _ItemProperties.relativeTransformNames.Add("Spine2");
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override Transform ItemParent()
        {
            return ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "Spine2");
        }

        private Transform FishRelativeTransform()
        {
            return ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "Spine2");
        }

        protected override void RegisterAnimCallbacks()
        {
            // todo: refactor fish extra
            var fish = _Objects["Fishing"].transform.Find("fish").gameObject;
            fish.transform.SetParent(FishRelativeTransform());
            // comment: fish parent changed, manage it by _Objects
            _Objects.Add(ItemProperties.Name+"fish", fish);
            fish.transform.localPosition = new Vector3();
            fish.transform.localRotation = Quaternion.identity;
            var fishmesher = fish.transform.GetComponentInChildren<SkinnedMeshRenderer>();
            fish.SetActive(false);

            var handle = _Objects["Fishing"].transform.Find("Handle").gameObject;
            var handleAnimator = handle.GetComponent<Animator>();
            handleAnimator.speed = 0;

            //todo: source Play() Stop()
            _SubStatus[0]._StatusAnimations[0].Events.SetCallback("fishing",
                () =>
                {
                    ExitEvent.Register(_SubStatus[0]._StatusAnimations[0].State, () =>
                    {
                        if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                        {
                            ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Stop();
                            Debug.Log("Item Events Fishing sound exit stop triggered");
                        }

                        if (ItemManager.FindItemByName("Fishing") == null)
                        {
                            return;
                        }

                        fish.SetActive(false);

                        handleAnimator.speed = 0;

                        _ItemProperties.ikTargetsDictionary[0] = new Dictionary<IKEffectorName, IKTarget>();
                        Debug.Log("lockarm fishing exit");
                        LockArmIK(0, true, true, true, true, 2);
                        LockArmIK(0, false, true, true, true, 2);
                    });

                    if (ItemManager.FindItemByName("Fishing") == null)
                    {
                        return;
                    }
                    fish.SetActive(true);
                    fishmesher.enabled = false;
                    handleAnimator.Play("Handle", -1, _SubStatus[0]._StatusAnimations[0].State.NormalizedTime);
                    handleAnimator.speed = 1.0f;
                    _ItemProperties.ikTargetsDictionary[0] = new Dictionary<IKEffectorName, IKTarget>();
                    Debug.Log("lockarm fishing setactive");
                    LockArmIK(0, true, false, false, false, 2);
                    LockArmIK(0, false, false, false, false, 2);
                }
                );
            _SubStatus[0]._StatusAnimations[0].Events.SetCallback("fishing_suc",
                () =>
                {
                    if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && !ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                    {
                        ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Play();
                        Debug.Log("Item Events Fishing sound play triggered");
                    }

                    if (ItemManager.FindItemByName("Fishing") == null)
                    {
                        return;
                    }
                    fishmesher.enabled = true;
                }
                );
            _SubStatus[0]._StatusAnimations[0].Events.SetCallback("fishing_suc2",
                () =>
                {
                    if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                    {
                        ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Stop();
                        Debug.Log("Item Events Fishing sound stop triggered");
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
                LockArmIK(slotIndex, false, true, true, true, 2);
                Debug.Log("Item Events Fishing SelfInactive triggered");
            });


            // Unlock right hand and poser when speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                LockArmIK(slotIndex, false, false, false, false, 2);
                Debug.Log("Item Events Fishing SelfSpeaking triggered");
            });
        }

        protected override void ExecuteExtraCmds()
        {
            
            Debug.Log("Item Events Fishing must triggered");
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            Debug.Log("lockarm fishing initialiktargets");
            LockArmIK(itemSlotIndex, true, true, true, true, 2);
            LockArmIK(itemSlotIndex, false, true, true, true, 2);
        }
    }
}