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
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/M_SkateBoard.prefab";
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_skateboard.wav";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_skateboard_baseIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_Body.mask");
            _ItemProperties.SecondIdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_walking_baseIdle_1.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.SecondIdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.SubStatusAnimPath.Add("Assets/Project/Prefabs/Item_skateboard_sub.prefab");
            _ItemProperties.SubStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_Body.mask");
            _ItemProperties.InteractionAnimPath.Add("Assets/Project/Prefabs/Item_skateboard_sub2.prefab");
            _ItemProperties.InteractionMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
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
            AddCameraRule(CameraTiming.Turnaround, 2, "skateboardMovingCam", "Assets/Project/Prefabs/vcam/vcam_skateboard_moving_first.prefab", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2, "skateboardCloseCam", "Assets/Project/Models/Cameras/skateboard_jinjing_second.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2, "skateboardSpeaker0Cam", "Assets/Project/Models/Cameras/skateboard_amytalk_third.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2, "skateboardSpeaker1Cam", "Assets/Project/Models/Cameras/skateboard_qingwatalk_fourth.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
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
