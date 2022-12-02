using Animancer;
using Cinemachine;
using Playa.App;
using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Playa.App.Cinemachine;
using Playa.Avatars;
using Playa.App.Actors;
using Playa.Common.Utils;

namespace Playa.Item
{
    public class Bed : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Bed";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/bed/model/props/bed2.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/bed/idle/LayingSleepAmy.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Arms_Only.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/bed/idle/LayingSleepQingwa.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Models/Arms_Only.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
        }

        protected override void RegisterChatEventCallbacks(int slotIndex)
        {
            Transform IKDollNodes = _Objects["Bed"].transform.Find("IKDollNodes" + slotIndex.ToString());

            // Unlock hand when speaking
            ItemEventManager.AddItemEventSelfSpeakingListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftElbow, new IKTarget(null, 0, 0, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightElbow, new IKTarget(null, 0, 0, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Chair SelfSpeaking triggered");
            });

            // Lock hand when not speaking
            ItemEventManager.AddItemEventSelfInactiveListener(this, slotIndex, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftElbow, new IKTarget(IKDollNodes.Find("IKDollNodesLeftElbow"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightElbow, new IKTarget(IKDollNodes.Find("IKDollNodesRightElbow"), 1, 1, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Chair SelfInactive triggered");
            });

            // Lock hand when silence
            ItemEventManager.AddItemEventAllInactiveListener(this, () =>
            {
                _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.LeftElbow, new IKTarget(IKDollNodes.Find("IKDollNodesLeftElbow"), 1, 1, 1));
                _ItemProperties.ikTargetsDictionary[slotIndex].Add(IKEffectorName.RightElbow, new IKTarget(IKDollNodes.Find("IKDollNodesRightElbow"), 1, 1, 1));
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[slotIndex]));
                Debug.Log("Item Events Chair AllInactive triggered");
            });

        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2,
                "bedMovingCam", "Assets/Items/bed/camera/YUANJING.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2,
                "bedCloseCam", "Assets/Items/bed/camera/PINGSHI.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2,
                "bedSpeaker0Cam", "Assets/Items/bed/camera/QINGWATALK.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2,
                "bedSpeaker1Cam", "Assets/Items/bed/camera/AMYTALK.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
        }

        protected override void ExecuteExtraCmds()
        {
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Active, ArmatureUtils.FindHead(OtherAvatarUsers[0].ActiveAvatarTransform).gameObject, 1, 0.35f, 0.045f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(OtherAvatarUsers[0], VoiceActivityType.Active, ArmatureUtils.FindHead(AffectAvatarUser.ActiveAvatarTransform).gameObject, 1, 0.35f, 0.045f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, ArmatureUtils.FindHead(OtherAvatarUsers[0].ActiveAvatarTransform).gameObject, 1, 0.35f, 0.045f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(OtherAvatarUsers[0], VoiceActivityType.Inactive, ArmatureUtils.FindHead(AffectAvatarUser.ActiveAvatarTransform).gameObject, 1, 0.35f, 0.045f));
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 1, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Chest, new IKTarget(IKDollNodes.Find("IKDollNodesChest"), 1, 1, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.LeftElbow, new IKTarget(IKDollNodes.Find("IKDollNodesLeftElbow"), 1, 1, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.RightElbow, new IKTarget(IKDollNodes.Find("IKDollNodesRightElbow"), 1, 1, 1));
        }
    }
}


