using Animancer;
using Cinemachine;
using Playa.App;
using Playa.App.Actors;
using Playa.Avatars;
using Playa.Common;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Playa.Item
{
    public class HandHoldItem_Standard : BaseItem
    {
        public UnityEvent conditionsIK;

        private bool ikTrigger = false;
        private bool _IKLocked = false;
        private string prefabName = "mobile";
        private Transform realLeftHand;

        public HandHoldItem_Standard(string _ItemName, string _TagName, SlotName slotName, string _ItemAssetPath)
        {
        }

        protected override void InitProperties()
        {
            _ItemProperties.Name = prefabName;
            _ItemProperties.Tags.Add("food");
            _ItemProperties.SlotNames[0] = new List<SlotName> { SlotName.Hand };
            realLeftHand = ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "LeftHand");
        }

        protected override void ExecuteCmds()
        {
            AvatarUser user = AffectAvatarUser;
            Transform userTransform = _ActorsUtils._ActorsManager.GetAvatarPosition(user);

            var cmd = new InstantiateObjectCmd(prefabName, "Assets/Items/phone/model/props/phone5.prefab", userTransform.position, userTransform.rotation, App.Stage.SpawnStrategy.Override);
            _StageUtils.ExecuteCmd(cmd);

            Transform IKDollNodes = _Objects[prefabName].transform.Find("IKDollNodes");
            Transform drinkLeftHandPoser = IKDollNodes.Find("IKDollNodesLeftHand");
            GameObject ikGoalLeftHand = Instantiate(drinkLeftHandPoser.gameObject);
            ikGoalLeftHand.name = "ikGoalLeftHand";

            ikGoalLeftHand.transform.position = drinkLeftHandPoser.position;
            ikGoalLeftHand.transform.rotation = drinkLeftHandPoser.rotation;

            InteractionObject objC = ikGoalLeftHand.GetComponent<InteractionObject>();

            // Lock left hand no matter what
            CreateikTargets(null);
            _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(user, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[AffectAvatarUserIndex]));


            Transform fakeLeftHand;

            fakeLeftHand = new GameObject("FakeLeftHand_mobile").transform;
            fakeLeftHand.parent = realLeftHand.parent;
            fakeLeftHand.localPosition = realLeftHand.localPosition;
            fakeLeftHand.localRotation = realLeftHand.localRotation;
            _Objects.Add("FakeLeftHand_mobile", fakeLeftHand.gameObject);

            ResetItemTransform(_Objects[prefabName].transform, IKDollNodes.Find("IKDollNodesLeftHand"), fakeLeftHand);

            ikGoalLeftHand.transform.parent =  ArmatureUtils.FindPartString(user.ActiveAvatarTransform, "Spine2");
            _Objects.Add("mobileIKGoal", ikGoalLeftHand);

            // Refactor so that animations could be unloaded
            var mobilePlay = Addressables.LoadAssetAsync<GameObject>
                ("Assets/Items/phone/idle/PhoneAvatarPrefab.prefab").WaitForCompletion();
            var mask0 = Addressables.LoadAssetAsync<AvatarMask>
                ("Assets/Project/Animations/Masks/Right_Arm_Locked.mask").WaitForCompletion();

            var sub_idle = new ItemIdleSubStatusGroup();
            sub_idle._StatusAnimations = new ClipTransition[0];
            _ActorsUtils.ExecuteCmd(new SetAvatarIdleSubStatusCmd(AffectAvatarUser, sub_idle, 0));

            var silence0 = new ItemSilenceStatusGroup();
            silence0._StatusAnimations = new ClipTransition[1];
            silence0._StatusAnimations[0] = mobilePlay.GetComponent<ClipTransitionPrefab>().ClipTransition;
            silence0._AvatarMasks = new AvatarMask[1];
            silence0._AvatarMasks[0] = mask0;
            _ActorsUtils.ExecuteCmd(new SetAvatarSilenceStatusCmd(user, silence0));

            var idle0 = new ItemIdleStatusGroup();
            idle0._StatusAnimations = new ClipTransition[1];
            idle0._StatusAnimations[0] = mobilePlay.GetComponent<ClipTransitionPrefab>().ClipTransition;
            var secondIdle0 = new ItemSecondIdleStatusGroup();
            secondIdle0._StatusAnimations = new ClipTransition[1];
            secondIdle0._StatusAnimations[0] = mobilePlay.GetComponent<ClipTransitionPrefab>().ClipTransition;
            _ActorsUtils.ExecuteCmd(new SetAvatarIdleStatusCmd(_BaseApp._AppStartupConfig.AvatarUsers[0], idle0, secondIdle0));

            conditionsIK = new UnityEvent();

            // Lock right hand poser no matter what
            //_ActorsUtils.ExecuteCmd(new AddIKInteractionCmd(conditionsIK, user, RootMotion.FinalIK.FullBodyBipedEffector.LeftHand, ikGoalLeftHand.transform));
            //ikTrigger = true;
            _IKLocked = true;
            Debug.Log("Item Events Mobile must");

            var clip = Addressables.LoadAssetAsync<AudioClip>(
                "Assets/Items/drink/sound/drink_water.wav").WaitForCompletion();
            ServiceLocator.AudioService.AddAudioToMixerGroup(ItemId, prefabName, clip, Audio.AudioGroup.Effect, false);

            ItemEventManager.AddItemEventSelfSpeakingListener(this, 0, () =>
            {
                //_ActorsUtils.ExecuteCmd(new RemoveIKInteractionByUserEffectorTypeCmd(user, RootMotion.FinalIK.FullBodyBipedEffector.LeftHand));
                _IKLocked = false;
                Debug.Log("Item Events Mobile SelfSpeaking triggered");
            });

            ItemEventManager.AddItemEventSelfInactiveListener(this, 0, () =>
            {
                //ikTrigger = true;
                _IKLocked = true;
                //Debug.Log("Item Events Mobile SelfInactive triggered");
            });

            silence0._StatusAnimations[0].Events.SetCallback("Texting",
                () =>
                {
                }
                );

            silence0._StatusAnimations[0].Events.SetCallback("Texting_off",
                () =>
                {
                }
                );
        }
        private void CreateikTargets(Transform IKDollNodes)
        {
            _ItemProperties.ikTargetsDictionary[AffectAvatarUserIndex].Add(IKEffectorName.LeftHand, new IKTarget(null, 0, 0, 2));
            _ItemProperties.ikTargetsDictionary[AffectAvatarUserIndex].Add(IKEffectorName.LeftElbow, new IKTarget(null, 0, 0, 2));
        }

        private void ResetItemTransform(Transform Item, Transform originalHand, Transform realHand)
        {
            originalHand.parent = realHand;
            Item.parent = originalHand;
            originalHand.localPosition = new Vector3();
            originalHand.localRotation = new Quaternion();
            Item.parent = realHand;
            originalHand.parent = Item;
        }

        void Update()
        {
            if (ikTrigger)
            {
                conditionsIK?.Invoke();
                ikTrigger = false;
            }
            _Objects["FakeLeftHand_mobile"].transform.localPosition = realLeftHand.localPosition;
            _Objects["FakeLeftHand_mobile"].transform.localRotation = realLeftHand.localRotation;
        }
    }
}