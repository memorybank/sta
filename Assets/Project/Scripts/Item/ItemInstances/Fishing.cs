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
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/M_fishing.prefab";
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
            var fish = _Objects[_ItemProperties.Name].transform.Find("M_fish").gameObject;
            fish.transform.SetParent(AffectAvatarUser.GetAvatarPosition());
            _Objects.Add(ItemProperties.Name + "fish", fish);
            fish.transform.localPosition = new Vector3();
            fish.transform.localRotation = Quaternion.identity;
            fish.SetActive(false);

            var objAimator = _Objects[_ItemProperties.Name].GetComponent<Animator>();
            //todo: source Play() Stop()
            _SubStatus[0]._StatusAnimations[0].Events.SetCallback("animStart",
                () =>
                {
                    ExitEvent.Register(_SubStatus[0]._StatusAnimations[0].State, () =>
                    {
                        if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                        {
                            ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Stop();
                            Debug.Log("Item Events Fishing sound exit stop triggered");
                        }

                        if (ItemManager.FindItemByName(_ItemProperties.Name) == null)
                        {
                            return;
                        }

                        objAimator.SetInteger("state", 0);

                        fish.SetActive(false);
                    });

                    if (ItemManager.FindItemByName(_ItemProperties.Name) == null)
                    {
                        return;
                    }
                    objAimator.SetInteger("state", 1);

                    fish.SetActive(true);

                    if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && !ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
                    {
                        ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Play();
                        Debug.Log("Item Events Fishing sound play triggered");
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
                LockArmIK(slotIndex, true, false, false, false, 2);
                Debug.Log("Item Events Fishing SelfInactive triggered");
            });


            // Unlock right hand and poser when speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                LockArmIK(slotIndex, true, true, true, true, 2);
                Debug.Log("Item Events Fishing SelfSpeaking triggered");
            });
        }
    }
}