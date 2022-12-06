using Animancer;
using Cinemachine;
using Playa.App;
using Playa.App.Cinemachine;
using Playa.Avatars;
using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa.App.Cinemachine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Playa.Item
{

    public class Ninja_Run : BaseItem
    {
        private Quaternion user0Rotation = Quaternion.Euler(0, 0f, 0);
        private Quaternion user1Rotation = Quaternion.Euler(0, 0f, 0);
        protected override void InitProperties()
        {
            OnboardOtherAvatars();
            _ItemProperties.Name = "Ninja_Run";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.Unique = true;
            _ItemProperties.SoundAssetPath = "Assets/Project/Audio/Streaming/Item_ninjarun.wav";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_ninjarun_baseIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_ninjarun_baseIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
            _ItemProperties.HasSpeed = true;
            _ItemProperties.Speed = 2.0f;
            //Empty Transform Slots
            //Todo: refactor this
            ItemSlotTransformDictionary[0] = null;
            ItemSlotTransformDictionary[1] = null;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            if (ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name) != null && !ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).isPlaying)
            {
                ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Stop();
                Debug.Log("Item Events Ninja_run sound stop triggered");
            }
            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];
            user0.RemovePrefabPositionRotation(ItemId);
            user1.RemovePrefabPositionRotation(ItemId);
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2,
                "ninja runMovingCam", "Assets/Project/Prefabs/vcam/vcam_ninjarun_moving_first.prefab", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2,
                "ninja runCloseCam", "Assets/Project/Models/Cameras/ninjarun_jinjing_second.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2,
                "ninja runSpeaker0Cam", "Assets/Project/Models/Cameras/ninjarun_amytalk_third.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2,
                "ninja runSpeaker1Cam", "Assets/Project/Models/Cameras/ninjarun_qingwatalk_fourth.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
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
            user1.SetPrefabPositionRotationToTarget(ItemId, (int)_ItemProperties.EffectArea, position, Quaternion.identity); Debug.Log("Item Events ninja_run must");
            ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Play();
            Debug.Log("Item Events Ninja_run sound play triggered");
        }
    }
}