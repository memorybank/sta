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
    public struct StageProperties
    {
        public string PlaneName;
        public string ObjectAssetPath;
        public string SoundAssetPath;
    }

public class StageItem : BaseItem
    {
        public float _UpDist = 5.0f;

        private Vector3 user0Position = new Vector3(0.5f, 0, 0);
        private Quaternion user0Rotation = Quaternion.Euler(0, -45f, 0);
        private Vector3 user1Position = new Vector3(-0.5f, 0, 0);
        private Quaternion user1Rotation = Quaternion.Euler(0, 45f, 0);

        protected StageProperties _StageProperties;
        public StageProperties StageProperties => _StageProperties;
        protected override void InitProperties()
        {
            OnboardOtherAvatars();
            _ItemProperties.Tags.Add("main_stage");
            _ItemProperties.Unique = true;
            _ItemProperties.EffectArea = ItemEffectArea.Stage;
            _ItemProperties.SlotNames[0] = new List<SlotName> { };
            _ItemProperties.SlotNames[1] = new List<SlotName> { };
            ItemSlotTransformDictionary[0] = null;
            ItemSlotTransformDictionary[1] = null;
        }

        protected override void ExecuteExtraCmds()
        {
            _StageUtils.ExecuteCmd(
               new InstantiateObjectCmd("grass", _StageProperties.ObjectAssetPath, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f), App.Stage.SpawnStrategy.Override));

            var path = _Objects["grass"].transform.Find("path");

            var plane = _Objects["grass"].transform.Find(StageProperties.PlaneName);
            var planeMesh = plane.GetComponent<MeshFilter>().mesh;
            plane.gameObject.AddComponent<MeshCollider>();
            plane.GetComponent<MeshCollider>().sharedMesh = planeMesh;
            Debug.Log("planemesh " + planeMesh.name);

            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];

            user0.SetPath(path);
            user1.SetPath(path);
            user0.SetPrefabPositionRotationToTarget(ItemId, (int)_ItemProperties.EffectArea, user0Position + new Vector3(0, _UpDist, 0), user0Rotation);
            user1.SetPrefabPositionRotationToTarget(ItemId, (int)_ItemProperties.EffectArea, user1Position + new Vector3(0, _UpDist, 0), user1Rotation);

            foreach (var user in _BaseApp._AppStartupConfig.AvatarUsers)
            {
                RaycastHit hit;
                Physics.Raycast(user.ActiveAvatarTransform.position, Vector3.down, out hit);
                Debug.Log("User " + user.ActiveAvatarTransform.position);
                Debug.Log("Hit " + hit.collider);
                Debug.Log("Hit " + hit.distance);

                var position = Vector3.zero;
                var rotation = Quaternion.identity;
                user.GetPrefabPositionRotationToTarget(ref position, ref rotation);
                position += new Vector3(0, -hit.distance, 0);
                user.SetPrefabPositionRotationToTarget(ItemId, (int)_ItemProperties.EffectArea, position, rotation);
            }

            // Update initial look at targets
            var head0 = ArmatureUtils.FindHead(user0.ActiveAvatarTransform);
            var head1 = ArmatureUtils.FindHead(user1.ActiveAvatarTransform);
            _BaseApp._AppStartupConfig.LookAtTargets[0].position = head0.position + user0.ActiveAvatarTransform.forward;
            _BaseApp._AppStartupConfig.LookAtTargets[1].position = head1.position + user1.ActiveAvatarTransform.forward;
            _BaseApp._CameraSwitchManager.OnInitItem(this);

            // Sound
            var stableMusic = Addressables.LoadAssetAsync<AudioClip>
                (_StageProperties.SoundAssetPath).WaitForCompletion();
            ServiceLocator.AudioService.AddAudioToMixerGroup(ItemId, _ItemProperties.Name, stableMusic, Audio.AudioGroup.Music, true);
            ServiceLocator.AudioService.GetAudioSource(ItemId, _ItemProperties.Name).Play();

            SetHeadIK();
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

        protected void SetHeadIK()
        {
            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];
            var config = _BaseApp._AppStartupConfig as StandAppStartupConfig;
            //SetHeadIK
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Active, ArmatureUtils.FindHead(user1.ActiveAvatarTransform).gameObject, 0, 1, 0.3f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Active, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 0, 1, 0.3f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user1.ActiveAvatarTransform).gameObject, 0, 1, 0.3f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Inactive, ArmatureUtils.FindHead(user0.ActiveAvatarTransform).gameObject, 0, 1, 0.3f));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user0, VoiceActivityType.Silence, config.LookAtTargets[0].gameObject, 0, 0, 0));
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user1, VoiceActivityType.Silence, config.LookAtTargets[1].gameObject, 0, 0, 0));
        }

        protected void LateUpdate()
        {
            var user0 = _BaseApp._AppStartupConfig.AvatarUsers[0];
            var user1 = _BaseApp._AppStartupConfig.AvatarUsers[1];

            user0.MultiIKManager.ManualUpdate();
            user1.MultiIKManager.ManualUpdate();

            var path = _Objects["grass"].transform.Find("path");
            var tweenPath = path.GetComponent<DOTweenPath>();
            DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);

            user0.ResetPrefabPositionRotationToTarget();
            user1.ResetPrefabPositionRotationToTarget();
        }
    }
}