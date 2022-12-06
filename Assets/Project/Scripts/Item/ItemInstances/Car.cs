using Animancer;
using Cinemachine;
using Playa.App;
using Playa.App.Actors;
using Playa.App.Cinemachine;
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
    public class Car : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();
            _ItemProperties.Name = "Car";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/M_Car.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_car.wav";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_car_baseIdle_0.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_car_baseIdle_1.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Tags.Add("vehicles");
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand, SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
            _ItemProperties.IKDollNodesTrackingSubItemDict.Add("LeftHand", "SM_Veh_Car_Small_SteeringW");
            _ItemProperties.IKDollNodesTrackingSubItemDict.Add("RightHand", "SM_Veh_Car_Small_SteeringW");
            _ItemProperties.HasSpeed = true;
            _ItemProperties.Speed = 3.0f;
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2, "houshiCam", "Assets/Project/Models/Cameras/car_houshi.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
        }

        protected override void ExecuteExtraCmds()
        {
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Active, ArmatureUtils.FindHead(OtherAvatarUsers[0].ActiveAvatarTransform).gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, ArmatureUtils.FindHead(OtherAvatarUsers[0].ActiveAvatarTransform).gameObject, 1, 0, 0));
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
            if (itemSlotIndex == 0)
            {
                _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftHand, new IKTarget(IKDollNodes.Find("IKDollNodesLeftHand"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightHand, new IKTarget(IKDollNodes.Find("IKDollNodesRightHand"), 1, 1, 1));
            }
        }
    }
}