using Animancer;
using Cinemachine;
using Playa.App;
using Playa.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Playa.Avatars;
using Playa.App.Actors;
using Playa.Common.Utils;
using Playa.App.Cinemachine;

namespace Playa.Item
{
    public class Movie : BaseItem
    {
        protected override void InitProperties()
        {
            OnboardOtherAvatars();

            _ItemProperties.Name = "Movie";
            _ItemProperties.EffectArea = ItemEffectArea.FullBody;
            _ItemProperties.ObjectAssetPath = "Assets/Project/Prefabs/SM_Movie.prefab";
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_movie_baseIdle_0.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.IdleStatusAnimPath.Add("Assets/Project/Prefabs/Item_movie_baseIdle_1.prefab");
            _ItemProperties.IdleStatusMaskPath.Add("Assets/Project/Animations/Masks/Head_And_Arms.mask");
            _ItemProperties.Unique = true;
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Body };
            _ItemProperties.SlotNames[1] = new List<SlotName> { SlotName.Body };
        }

        protected override void RegisterAdditionalCameras()
        {
            AddCameraRule(CameraTiming.Turnaround, 2, "movieStartCam", "Assets/Project/Models/Cameras/movie_pingshi_second.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.AllInactive, 2, "movieSilenceCam", "Assets/Project/Models/Cameras/movie_yuanjing_first.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker0, 2, "movieSpeaker0Cam", "Assets/Project/Models/Cameras/movie_amytalk_third.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
            AddCameraRule(CameraTiming.Speaker1, 2, "movieSpeaker1Cam", "Assets/Project/Models/Cameras/movie_qingwatalk_foutth.fbx", ItemManager.FindStageItem()._Objects["grass"].transform);
        }

        protected override void ExecuteExtraCmds()
        {
            var user0 = ItemSlotUserDictionary[0].AvatarUser;
            var user1 = ItemSlotUserDictionary[1].AvatarUser;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Active, ArmatureUtils.FindHead(user1.ActiveAvatarTransform).gameObject, 1, 0.8f, 0.2f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Active, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 1, 0.7f, 0.2f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user1.ActiveAvatarTransform).gameObject, 1, 0.7f, 0.1f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 1, 0.6f, 0.1f));
        }

        protected override void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[itemSlotIndex].Add(IKEffectorName.Root, new IKTarget(IKDollNodes.Find("IKDollNodesHip"), 1, 0, 1));
        }
    }
}


