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
    public class Drinking : BaseItem
    {        
        protected override void InitProperties()
        {
            _ItemProperties.Name = "Drinking";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Items/drink/model/props/coffee3.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Items/drink/sound/drink_water.wav";
            _ItemProperties.SubStatusAnimPath.Add("Assets/Items/drink/idle/DrinkingClipPrefab.prefab");
            _ItemProperties.SubStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Left_Arm.mask");
            _ItemProperties.Tags.Add("food");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            Dictionary<string, GameObject> d = new();
            d.Add("LeftHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "LeftHand")
                );
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }

        protected override void RegisterAnimCallbacks()
        {
            _SubStatus[0]._StatusAnimations[0].Events.SetCallback("drinking",
                () =>
                {
                    ExitEvent.Register(_SubStatus[0]._StatusAnimations[0].State, () =>
                    {
                        if (_IKHandLocked) return;
                        if (IKGoalDict[0]["LeftHand"] == null) return;

                        _ItemProperties.ikTargetsDictionary[0] = new Dictionary<IKEffectorName, IKTarget>();
                        _ItemProperties.ikTargetsDictionary[0].Add(IKEffectorName.LeftHand, new IKTarget(IKGoalDict[0]["LeftHand"].transform, 1, 1, 2));
                        _ItemProperties.ikTargetsDictionary[0].Add(IKEffectorName.LeftHandPoser, new IKTarget(IKGoalDict[0]["LeftHand"].transform, 1, 1, 2));
                        _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(AffectAvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[0]));

                        _IKHandLocked = true;
                        Debug.Log("Item Events Drinking exited");
                    });

                    _ItemProperties.ikTargetsDictionary[0] = new Dictionary<IKEffectorName, IKTarget>();
                    _ItemProperties.ikTargetsDictionary[0].Add(IKEffectorName.LeftHand, new IKTarget(null, 0, 0, 2));
                    _ItemProperties.ikTargetsDictionary[0].Add(IKEffectorName.LeftHandPoser, new IKTarget(null, 0, 0, 2));
                    _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(AffectAvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[0]));

                    ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Play();
                    _IKHandLocked = false;
                    Debug.Log("Item Events Drinking started");
                }
                );

            _SubStatus[0]._StatusAnimations[0].Events.SetCallback("drinking_off",
                () =>
                {
                    if (_IKHandLocked) return;
                    if (IKGoalDict[0]["LeftHand"] == null) return;

                    _ItemProperties.ikTargetsDictionary[0] = new Dictionary<IKEffectorName, IKTarget>();
                    _ItemProperties.ikTargetsDictionary[0].Add(IKEffectorName.LeftHand, new IKTarget(IKGoalDict[0]["LeftHand"].transform, 1, 1, 2));
                    _ItemProperties.ikTargetsDictionary[0].Add(IKEffectorName.LeftHandPoser, new IKTarget(IKGoalDict[0]["LeftHand"].transform, 1, 1, 2));
                    _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(AffectAvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[0]));

                    _IKHandLocked = true;
                    Debug.Log("Item Events Drinking finished");
                }
                );
        }

        protected override void ExecuteExtraCmds()
        {
            _IKHandLocked = true;
            Debug.Log("Item Events Drinking must triggered");
        }
        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            // Lock left hand no matter what
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftElbow, new IKTarget(null, 0, 0, 2));
            var leftHand = IKGoalDict[0]["LeftHand"] == null ? IKDollNodes.Find("IKDollNodesLeftHand") : IKGoalDict[0]["LeftHand"].transform;
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftHand, new IKTarget(leftHand, 1, 1, 2));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftHandPoser, new IKTarget(leftHand, 1, 1, 2));
        }
    }
}