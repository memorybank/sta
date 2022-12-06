using Animancer;
using Cinemachine;
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

    public class Walking : BaseItem
    {
        private Quaternion user0Rotation = Quaternion.Euler(0, 0f, 0);
        private Quaternion user1Rotation = Quaternion.Euler(0, 0f, 0);

        protected override void InitProperties()
        {
            OnboardOtherAvatars();
            _ItemProperties.Name = "Walking";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.Unique = true;
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_walking_baseIdle_0.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_walking_baseIdle_1.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.SlotNames[0] = new List<SlotName>{ SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName>{ SlotName.Body };
            _ItemProperties.HasSpeed = true;
            _ItemProperties.Speed = 0.5f;
            //Empty Transform Slots
            //Todo: refactor this
            ItemSlotTransformDictionary[0] = null;
            ItemSlotTransformDictionary[1] = null;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];
            user0.RemovePrefabPositionRotation(ItemId);
            user1.RemovePrefabPositionRotation(ItemId);
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2, "walkingMovingCam", "Assets/Project/Prefabs/vcam/vcam_walking_moving_first.prefab", 
                ItemManager.FindStageItem()._Objects["grass"].transform);
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

            Debug.Log("Item Events walking must");
        }
    }
}