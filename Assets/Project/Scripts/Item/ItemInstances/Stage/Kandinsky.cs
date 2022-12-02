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
using DG.Tweening;

namespace Playa.Item
{

    public class Kandinsky : StageItem
    {
        protected override void InitProperties()
        {
            base.InitProperties();
            _ItemProperties.Name = "Kandinsky";
            _ItemProperties.IsMovable = true;
            _StageProperties.ObjectAssetPath = "Assets/Items/stage/kandinsky.prefab";
            _StageProperties.SoundAssetPath = "Assets/Items/stable/sound/2.wav";
            _StageProperties.PlaneName =  "daolu_zong";
        }

        protected override void RegisterAdditionalCameras()
        {
            var grass = _Objects["grass"].transform;

            // speaking close cam
            AddCameraRule(CameraTiming.AllInactive, 1, "grassSpeakingCloseCam", "Assets/Items/stable/camera/vcam_pingshi.prefab", grass);

            // speaker0 cam
            AddCameraRule(CameraTiming.Speaker0, 1, "grassAvatar0SpeakingCloseCam", "Assets/Items/stable/camera/vcam_amy_talk.prefab",
                grass, ArmatureUtils.FindHead(_BaseApp._AppStartupConfig.AvatarUsers[0].ActiveAvatarTransform));

            // speaker1 cam
            AddCameraRule(CameraTiming.Speaker1, 1, "grassAvatar1SpeakingCloseCam", "Assets/Items/stable/camera/vcam_qingwa_talk.prefab",
                grass, ArmatureUtils.FindHead(_BaseApp._AppStartupConfig.AvatarUsers[1].ActiveAvatarTransform));

            // speaking turnaround cam
            AddCameraRule(CameraTiming.Turnaround, 1, "grassCloseCam", "Assets/Items/stable/camera/vcam_yuanjing.prefab", grass);

        }



        protected override void OnAvatarSpeedChange(bool isSelfChange, float speed)
        {
            Debug.Log("Kandinsky on avatar speed change " + isSelfChange + " s " + speed);
            if (isSelfChange)
            {
                var path = _Objects["grass"].transform.Find("path");
                var tweenPath = path.GetComponent<DOTweenPath>();

                if (speed == 0f)
                {
                    tweenPath.DOPause();
                }
                else
                {                    
                    DOTween.timeScale = speed;
                    tweenPath.DOPlay();
                }
            }
        }
    }
}