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

    public class Biking : BaseItem
    {
        public Vector3 _CloseCameraPosition = new Vector3(-1.0f, 8.0f, 14.0f);
        public Quaternion _CloseCameraQuaternion = Quaternion.Euler(25.0f, 175.0f, 0.0f);

        private Animator _BikeAnimator;
        private List<ReferenceInfo> _BikeMotionInfo;
        private Transform _LeftFoot;

        private Quaternion userRotation = Quaternion.Euler(0, 0f, 0);

        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Biking";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Items/bike/model/props/bike7.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/bike/idle/biking.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Full_NO.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Items/bike/idle/bike_sit.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand, SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
            _ItemProperties.IsMovable = true;
            _ItemProperties.HasSpeed = true;
            _ItemProperties.Speed = 2.0f;
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2, "bikingMovingCam", "Assets/Items/bike/camera/MOVING_first.prefab", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2, "bikingCloseCam", "Assets/Items/bike/camera/PINGSHI_second.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2, "bikingSpeaker0Cam", "Assets/Items/bike/camera/AMYTALK_third.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2, "bikingSpeaker1Cam", "Assets/Items/bike/camera/QINGWATALK_fourth.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
        }

        protected override void ExecuteExtraCmds()
        {
            var user0 = ItemSlotUserDictionary[0].AvatarUser;
            var user1 = ItemSlotUserDictionary[1].AvatarUser;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Active, ArmatureUtils.FindHead(user1.ActiveAvatarTransform).gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Active, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 1, 0.6f, 0.2f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user1.ActiveAvatarTransform).gameObject, 1, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 1, 0.6f, 0.2f));
            // Motion matching for bike
            _BikeAnimator = _Objects[_ItemProperties.Name].GetComponent<Animator>();
            _BikeAnimator.playableGraph.SetTimeUpdateMode(UnityEngine.Playables.DirectorUpdateMode.Manual);

            var panel = GameUtils.FindDeepChild(_Objects[_ItemProperties.Name].transform, "pedal_l");

            _BikeMotionInfo = MotionExtracter.ExtractReference(_BikeAnimator, panel, 100);
            _LeftFoot = ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "LeftToeEnd");
        }

        private void LateUpdate()
        {
            // todo: manual update by stage item
            if (_BikeMotionInfo == null)
            {
                return;
            }
            MotionMatcher.MatchByRefernce(_BikeAnimator, _BikeMotionInfo, _LeftFoot);
        }
        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Chest, new IKTarget(IKDollNodes.Find("IKDollNodesChest"), 1, 0, 1));
            LockArmIK(itemSlotIndex, true, true, false, true, 1);
            LockArmIK(itemSlotIndex, false, true, false, true, 1);
        }

    }
}