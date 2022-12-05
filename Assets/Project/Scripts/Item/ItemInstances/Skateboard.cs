using Animancer;
using Cinemachine;
using Playa.Animations;
using Playa.App;
using Playa.App.Cinemachine;
using Playa.Avatars;
using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
namespace Playa.Item
{
    public class Skateboard : BaseItem
    {
        private Quaternion user0Rotation = Quaternion.Euler(0, 0f, 0);
        private Quaternion user1Rotation = Quaternion.Euler(0, 0f, 0);
        protected override void InitProperties()
        {
            OnboardOtherAvatars();
            _ItemProperties.Name = "Skateboard";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/skateboard/model/props/skateboard_yougujia.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Items/skateboard/sound/zapsplat_sport_skateboard_plastic_short_roll_on_street_004_30614.wav";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/skateboard/idle/skateIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/ActorAnimation/Man_Walking.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
            _ItemProperties.HasSpeed = true;
            _ItemProperties.Speed = 0.5f;
            ItemSlotTransformDictionary[0] = null;
            ItemSlotTransformDictionary[1] = null;
        }
        public override void Deactivate()
        {
            base.Deactivate();
            if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && !ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
            {
                ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Stop();
                Debug.Log("Item Events Skateboard sound stop triggered");
            }
            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];
            user0.RemovePrefabPositionRotation(ItemId);
            user1.RemovePrefabPositionRotation(ItemId);
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2, "skateboardMovingCam", "Assets/Items/skateboard/camera/MOVING_first.prefab", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2, "skateboardCloseCam", "Assets/Items/skateboard/camera/JINJING_second.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2, "skateboardSpeaker0Cam", "Assets/Items/skateboard/camera/AMYTALK_third.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2, "skateboardSpeaker1Cam", "Assets/Items/skateboard/camera/QINGWATALK_fourth.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            //_ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftFoot, new IKTarget(IKDollNodes.Find("IKDollNodesLeftFoot"), 1, 1, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightFoot, new IKTarget(IKDollNodes.Find("IKDollNodesRightFoot"), 1, 1, 1));
        }

        protected override void ExecuteExtraCmds()
        {
            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];

            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            user0.GetPrefabPositionRotationToTarget(ref position, ref rotation);
            user0.SetPrefabPositionRotationToTarget(ItemId, (int)_ItemProperties.EffectArea, position, Quaternion.identity);
            user1.GetPrefabPositionRotationToTarget(ref position, ref rotation);
            user1.SetPrefabPositionRotationToTarget(ItemId, (int)_ItemProperties.EffectArea, position, Quaternion.identity);

            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Inactive, _BaseApp._AppStartupConfig.LookAtTargets[0].gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Active, _BaseApp._AppStartupConfig.LookAtTargets[0].gameObject, 1, 0, 0));

            Debug.Log("Item Events Skateboard must triggered");
        }
    }
}
