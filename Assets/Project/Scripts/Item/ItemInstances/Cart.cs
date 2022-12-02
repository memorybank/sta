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

    public class Cart : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Cart";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/cart/model/origin/Cart.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/cart/model/origin/PushCart.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Head_Only.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/cart/model/origin/lieIdle.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Head_And_Arms.mask");
            _ItemProperties.SubStatusAnimPath.Add("Assets/Items/cart/model/origin/LieonCart.prefab");
            _ItemProperties.SubStatusMaskPath.Add("Assets/Models/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand, SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
            _ItemProperties.IsMovable = true;
            _ItemProperties.HasSpeed = true;
            _ItemProperties.Speed = 0.5f;
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2,
                "CartMovingCam", "Assets/Items/cart/camera/MOVING_first.prefab", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2,
                "CartCloseCam", "Assets/Items/cart/camera/PINGSHI_second.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2,
                "CartSpeaker0Cam", "Assets/Items/cart/camera/AMYTALK_third.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2,
                "CartSpeaker1Cam", "Assets/Items/cart/camera/QINGWATALK_fhourth.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
        }

        protected override void ExecuteExtraCmds()
        {
            var user0 = ItemSlotUserDictionary[0].AvatarUser;
            var user1 = ItemSlotUserDictionary[1].AvatarUser;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Active, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 1, 0f, 0f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 1, 0f, 0f));
        }
    }
}