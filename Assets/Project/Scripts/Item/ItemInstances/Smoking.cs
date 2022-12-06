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
    public class Smoking : BaseItem
    {

        protected override void InitProperties()
        {
            _ItemProperties.Name = "Smoking";
            _ItemProperties.EffectArea = ItemEffectArea.HandHold;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/SM_Cigratte.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_smoking_sub.wav";
            _ItemProperties.InteractionAnimPath.Add("Assets/Project/Prefabs/Item_smoking_sub.prefab");
            _ItemProperties.InteractionMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Right_Arm.mask");
            _ItemProperties.SilenceStatusAnimPath.Add("Assets/Project/Prefabs/Item_smoking_sub.prefab");
            _ItemProperties.SilenceStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Right_Arm.mask");
            _ItemProperties.Tags.Add("food");
            _ItemProperties.relativeTransformNames.Add("Spine2");
            Dictionary<string, GameObject> d = new();
            d.Add("RightHand", null);
            _ItemProperties.fakeOrFollows.Add(
                new ItemProperties.FakeOrFollow(d, "RightHand")
                );
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
        }
        protected override void ExecuteExtraCmds()
        {
            _IKHandLocked = true;
            Debug.Log("Item Events Drinking must triggered");
        }
        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            var rightHand = IKGoalDict[itemSlotIndex]["RightHand"] == null ? IKDollNodes.Find("IKDollNodesRightHand") : IKGoalDict[itemSlotIndex]["RightHand"].transform;
            //_ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightHand, new IKTarget(rightHand, 1, 1, 2));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightHandPoser, new IKTarget(rightHand, 1, 1, 2));
        }
    }
}