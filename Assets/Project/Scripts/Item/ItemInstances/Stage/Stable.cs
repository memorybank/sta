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

    public class Stable : StageItem
    {
        protected override void InitProperties()
        {
            base.InitProperties();
            _ItemProperties.Name = "Stable";
            _ItemProperties.IsMovable = true;
            _StageProperties.ObjectAssetPath = "Assets/Items/GroundItem/grass_moving4.prefab";
            _StageProperties.SoundAssetPath = "Assets/Items/stable/sound/2.wav";
            _StageProperties.PlaneName = "sphere";
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

        protected override void OnAvatarSpeedChange(bool isSelfChange, float speed)
        {
            if (isSelfChange)
            {
                PlayMovableAnimationBySpeed("grass", "Scene", 50.0f / 180.0f, speed);
            }
        }
    }
}