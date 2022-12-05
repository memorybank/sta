using System.Collections;
using System.Collections.Generic;
using Timer = Playa.Common.Utils.Timer;
using Cinemachine;
using Playa.App;
using Playa.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using Playa.Avatars;
using Playa.App.Cinemachine;

namespace Playa.Item
{

    public class House : StageItem
    {
        protected override void InitProperties()
        {
            base.InitProperties();
            _ItemProperties.Name = "House";
            _ItemProperties.IsMovable = false;
            _StageProperties.ObjectAssetPath = "Assets/Project/Prefabs/P_70s_House.prefab";
            //_StageProperties.SoundAssetPath = "Assets/Items/stable/sound/2.wav";
            _StageProperties.PlaneName = "SM_House";
        }

        protected override void RegisterAdditionalCameras()
        {
            var grass = _Objects["grass"].transform;

            // speaking close cam
            _CinemachineUtils.ExecuteCmd(new AddCameraRuleCmd(CameraTiming.AllInactive,
                1, "grassSpeakingCloseCam", "Assets/Project/Prefabs/vcam/vcam_pingshi.prefab", grass));

            // speaking close cam
            _CinemachineUtils.ExecuteCmd(new AddCameraRuleCmd(CameraTiming.Speaker0,
                1, "grassAvatar0SpeakingCloseCam", "Assets/Project/Prefabs/vcam/vcam_amy_talk.prefab", grass, ArmatureUtils.FindHead(_BaseApp._AppStartupConfig.AvatarUsers[0].ActiveAvatarTransform)));

            // speaking close cam
            _CinemachineUtils.ExecuteCmd(new AddCameraRuleCmd(CameraTiming.Speaker1,
                1, "grassAvatar1SpeakingCloseCam", "Assets/Project/Prefabs/vcam/vcam_qingwa_talk.prefab", grass, ArmatureUtils.FindHead(_BaseApp._AppStartupConfig.AvatarUsers[1].ActiveAvatarTransform)));

            // speaking turnaround cam
            _CinemachineUtils.ExecuteCmd(new AddCameraRuleCmd(CameraTiming.Turnaround,
                1, "grassCloseCam", "Assets/Project/Prefabs/vcam/vcam_yuanjing.prefab", _Objects["grass"].transform));

        }
    }
}