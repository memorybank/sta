using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.App;
using System;
using Cinemachine;
using UnityEngine.Events;
using Animancer;
using Playa.Common;
using Playa.Avatars;
using RootMotion.FinalIK;
using UnityEngine.AddressableAssets;
using Protos;
using Playa.Common.Utils;
using Playa.App.Cinemachine;

namespace Playa.Item
{
    using ItemId = UInt64;
    using InteractionId = UInt64;

    public enum ItemEffectArea
    {
        NA = 0,
        Stage = 100,
        Place = 200,
        FullBody = 300,
        HandHold = 400,        
    }

    public struct ItemProperties
    {
        public struct FakeOrFollow
        {
            public Dictionary<string, GameObject> FakeNames;
            public string FollowObject;

            public FakeOrFollow(Dictionary<string, GameObject> fakeNames, string followObject)
            {
                FakeNames = fakeNames;
                FollowObject = followObject;
            } 
        }
        public string Name;
        public string ObjectAssetPath;
        public string SoundAssetPath;
        public ItemEffectArea EffectArea;
        public List<string> IdleStatusAnimPath;
        public List<string> IdleStatusMaskPath;
        public List<string> SecondIdleStatusMaskPath;
        public List<string> SubStatusAnimPath;
        public List<string> SubStatusMaskPath;
        public List<string> SilenceStatusAnimPath;
        public List<string> SilenceStatusMaskPath;
        public List<string> ActionStatusAnimPath;
        public List<string> ActionStatusMaskPath;
        public List<string> InteractionAnimPath;
        public List<string> InteractionMaskPath;
        public List<string> Tags;
        public List<string> relativeTransformNames;
        public List<FakeOrFollow> fakeOrFollows;
        public Dictionary<string, string> IKDollNodesTrackingSubItemDict;
        public bool NeedConfirm;
        public bool Unique;
        public bool IsMovable;
        public bool SoundCycle;
        public bool HasSpeed;
        public float Speed;
        public EulerAngleDeformerOptions EulerAngleDeformerOptions;
        // item slot to avatar slots
        public Dictionary<int, List<SlotName>> SlotNames;
        // avatar to avatar iks
        public Dictionary<int, Dictionary<IKEffectorName, IKTarget>> ikTargetsDictionary;
    }


    public class BaseItem : ItemApi
    {
        public class AvatarUserIndexBundle
        {
            public AvatarUser AvatarUser;
            public int Index;

            public AvatarUserIndexBundle(AvatarUser _AvatarUser, int _Index)
            {
                AvatarUser = _AvatarUser;
                Index = _Index;
            }
        }

        public BaseApp _BaseApp;
        // Event manager for behavior injection
        public ItemEventManager ItemEventManager => _BaseApp._ItemEventManager;

        // Services and utils that item relies on to manipulate state
        protected StageUtils _StageUtils;
        protected CinemachineUtils _CinemachineUtils;
        protected ActorsUtils _ActorsUtils;
        public ItemManager ItemManager => _BaseApp._ItemManager;
        public ServiceLocator ServiceLocator => _BaseApp._ServiceLocator;

        public StageUtils StageUtils => _StageUtils;
        public CinemachineUtils CinemachineUtils => _CinemachineUtils;
        public ActorsUtils ActorsUtils => _ActorsUtils;

        public Dictionary<int, Transform> ItemSlotTransformDictionary = new();
        public Dictionary<int, AvatarUserIndexBundle> ItemSlotUserDictionary = new();

        public struct InteractionResistry
        {
            public UnityEvent condition;
            public UnityAction action;
        }

        // Internal state
        protected ItemProperties _ItemProperties;
        public ItemProperties ItemProperties => _ItemProperties;

        public bool _IsActivated;

        protected Dictionary<string, InteractionResistry> _Interactions;

        private ItemId _ItemId;

        protected bool _IKHandLocked = false;

        protected List<ItemIdleStatusGroup> _IdleStatus;
        protected List<ItemSecondIdleStatusGroup> _SecondIdleStatus;
        protected List<ItemIdleSubStatusGroup> _SubStatus;
        protected List<ItemSilenceStatusGroup> _SilenceStatus;
        protected List<ItemActionStatusGroup> _ActionStatus;
        protected List<ItemInteractionStatusGroup> _InteractionStatus;

        public List<Dictionary<string, GameObject>> IKGoalDict;

        public Dictionary<string, GameObject> _Objects;

        // Icon
        public ItemId ItemId => _ItemId;

        public List<InteractionId> _InteractionIds;

        public AvatarUser AffectAvatarUser;
        public int AffectAvatarUserIndex;

        public List<AvatarUser> OtherAvatarUsers = new();
        public List<int> OtherAvatarUserIndices = new();

        private void Awake()
        {
            _Objects = new Dictionary<string, GameObject>();
            IKGoalDict = new List<Dictionary<string, GameObject>>();
            _Interactions = new Dictionary<string, InteractionResistry>();
            _InteractionIds = new List<InteractionId>();
            _IdleStatus = new List<ItemIdleStatusGroup>();
            _SecondIdleStatus = new List<ItemSecondIdleStatusGroup>();
            _SubStatus = new List<ItemIdleSubStatusGroup>();
            _SilenceStatus = new List<ItemSilenceStatusGroup>();
            _ActionStatus = new List<ItemActionStatusGroup>();
            _InteractionStatus = new List<ItemInteractionStatusGroup>();

            _ItemProperties.Name = "";
            _ItemProperties.EffectArea = ItemEffectArea.NA;
            _ItemProperties.Tags = new List<string>();
            _ItemProperties.IdleStatusAnimPath = new List<string>();
            _ItemProperties.IdleStatusMaskPath = new List<string>();
            _ItemProperties.SecondIdleStatusMaskPath = new List<string>();
            _ItemProperties.SubStatusAnimPath = new List<string>();
            _ItemProperties.SubStatusMaskPath = new List<string>();
            _ItemProperties.SilenceStatusAnimPath = new List<string>();
            _ItemProperties.SilenceStatusMaskPath = new List<string>();
            _ItemProperties.ActionStatusAnimPath = new List<string>();
            _ItemProperties.ActionStatusMaskPath = new List<string>();
            _ItemProperties.InteractionAnimPath = new List<string>();
            _ItemProperties.InteractionMaskPath = new List<string>();
            _ItemProperties.relativeTransformNames = new List<string>();
            _ItemProperties.SlotNames = new Dictionary<int, List<SlotName>>();
            _ItemProperties.SlotNames[0] = new List<SlotName> { };
            _ItemProperties.fakeOrFollows = new List<ItemProperties.FakeOrFollow>();
            _ItemProperties.IKDollNodesTrackingSubItemDict = new Dictionary<string, string>();
            _ItemProperties.SoundCycle = true;
            _ItemProperties.HasSpeed = false;
            _ItemProperties.Speed = 0;
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (_ItemProperties.InteractionAnimPath.Count > 0)
                {
                    AffectAvatarUser.GestureBehaviorPlanner.AvatarLayeredAnimationManager.SetInteractionLayerMask(
                        _InteractionStatus[AffectAvatarUserIndex]._AvatarMasks[0]);
                    AffectAvatarUser.GestureBehaviorPlanner.AvatarLayeredAnimationManager.PlayInteraction(
                        _InteractionStatus[AffectAvatarUserIndex]._StatusAnimations[0], UserStateTransitionConstants.InteractionFastFadeDuration, FadeMode.FromStart);
                }
            }
        }

        public void ActivateByUser(string uuid)
        {
            AvatarUser user = null;
            for(int i=0;i < _BaseApp._AppStartupConfig.AvatarUsers.Length;i++)
            {
                Debug.Log("pitaya we are here activate client " + _BaseApp._AppStartupConfig.AvatarUsers[i].AvatarUUID.ToString() + " " + uuid);
                if (_BaseApp._AppStartupConfig.AvatarUsers[i].AvatarUUID.ToString() == uuid)
                {
                    user = _BaseApp._AppStartupConfig.AvatarUsers[i];
                    AffectAvatarUser = user;
                    AffectAvatarUserIndex = i;
                    break;
                }
            }

            if (user == null)
            {
                Debug.Log("pitaya we are here user null");
                return;
            }

            InitProperties();
            if (Validate())
            {
                _ItemId = ItemManager.Register(this, user);
                PreActivate();
                ExecuteCmds();
                SetProperties();
                PostActivate();
            }
        }

        public virtual void Deactivate()
        {
            Debug.Log("Deactive " + ItemId);
            if (!_IsActivated)
            {
                return;
            }

            // base deactivate
            _IsActivated = false;

            ItemEventManager.RemoveAllListenersByItem(this);

            ServiceLocator.AudioService.ClearAllAudioForItem(_ItemId);

            _CinemachineUtils.ExecuteCmd(new RemoveCameraRuleCmd());

            CleanProperties();
            RemoveAvatarStatusAnimations();
            ResetAvatarPositions();
            ClearItemSlotTargets();

            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Active, null, -1, 0f, 0f)) ;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Inactive, null, -1, 0f, 0f)) ;
            _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(AffectAvatarUser, VoiceActivityType.Silence, null, -1, 0f, 0f)) ;
            foreach (var user in OtherAvatarUsers)
            {
                _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user, VoiceActivityType.Active, null, -1, 0f, 0f));
                _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user, VoiceActivityType.Inactive, null, -1, 0f, 0f));
                _ActorsUtils.ExecuteCmd(new SetLookAtIKCmd(user, VoiceActivityType.Silence, null, -1, 0f, 0f));
            }

            AffectAvatarUser.MultiIKManager.Deformer.SetOptions(EulerAngleDeformer.CreateDefaultOptions());
            AffectAvatarUser.MultiIKManager.RemoveFollowers(ItemId);

            foreach (var kV in _Objects)
            {
                Debug.Log("Deactivate destory object name " + kV.Key);
                Destroy(kV.Value);
            }

            var gameObject = ItemManager.FindItemById(ItemId);
            ItemManager.DeactivateItemName(_ItemProperties.Name);
            Destroy(gameObject);
        }

        protected virtual void InitProperties() { }

        protected void OnboardOtherAvatars()
        {
            for (int i = 0; i < _BaseApp._AppStartupConfig.AvatarUsers.Length; i++)
            {
                if (OtherAvatarUserIndices.Contains(i))
                {
                    continue;
                }

                if (i == AffectAvatarUserIndex)
                {
                    continue;
                }
                OtherAvatarUserIndices.Add(i);
                OtherAvatarUsers.Add(_BaseApp._AppStartupConfig.AvatarUsers[i]);
            }
        }

        protected void PreActivate()
        {
            _StageUtils = new StageUtils(this);
            _CinemachineUtils = new CinemachineUtils(this);
            _ActorsUtils = new ActorsUtils(this);

            _ItemProperties.ikTargetsDictionary = new();
            for (int i = 0; i < _BaseApp._AppStartupConfig.AvatarUsers.Length; i++)
            {
                _ItemProperties.ikTargetsDictionary.Add(i, new Dictionary<IKEffectorName, IKTarget>());
            }

            if (_ItemProperties.SlotNames[0].Count > 0)
            {
                _ActorsUtils.ExecuteCmd(new RegisterAvatarItemSlotCmd(AffectAvatarUser, _ItemProperties.SlotNames[0], _ItemProperties.ikTargetsDictionary[0]));
            }

            Debug.Assert(OtherAvatarUserIndices.Count < ItemProperties.SlotNames.Count, "Not enough slots");
            for (int i = 0; i < OtherAvatarUserIndices.Count; i++)
            {
                _ActorsUtils.ExecuteCmd(new RegisterAvatarItemSlotCmd(OtherAvatarUsers[i], ItemProperties.SlotNames[i + 1], _ItemProperties.ikTargetsDictionary[i+1]));
            }
        }

        protected void ClearItemSlotTargets()
        {
            if (_ItemProperties.SlotNames[0].Count > 0)
            {
                _ActorsUtils.ExecuteCmd(new ClearAvatarItemSlotCmd(AffectAvatarUser, _ItemProperties.SlotNames[0]));
            }

            Debug.Assert(OtherAvatarUserIndices.Count < ItemProperties.SlotNames.Count, "Not enough slots");
            for (int i = 0; i < OtherAvatarUserIndices.Count; i++)
            {
                _ActorsUtils.ExecuteCmd(new ClearAvatarItemSlotCmd(OtherAvatarUsers[i], ItemProperties.SlotNames[i + 1]));
            }
        }

        protected void PostActivate()
        {
            foreach (var kvp in ItemEventManager.eventSequencerManager.AvatarBrains)
            {
                kvp.Value.AvatarUser.HeadIKManager.ExcuteHeadIK(ItemEventManager.eventSequencerManager.AudioStates[kvp.Key]);
            }
            // Repopulate current events
            ItemEventManager.RepopulateEvents(this);

            _IsActivated = true;
        }

        protected virtual bool Validate()
        {
            if (_ItemProperties.NeedConfirm)
            {
                // yield for other user confirm without timeout
            }

            if (_ItemProperties.Unique)
            {
                var item = ItemManager.FindItemByName(_ItemProperties.Name);
                if (item != null)
                {
                    return false;
                }
            }

            return true;
        }

        // Methods that can override activate behavior
        protected virtual Vector3 ItemPositionFromUser(AvatarUser user)
        {
            Debug.Log("ItemPositionFromUser " + user.GetAvatarPosition().position);
            return user.GetAvatarPosition().position;
        }

        protected virtual Quaternion ItemRotationFromUser(AvatarUser user)
        {
            return user.GetPath().localRotation;
        }

        protected virtual Transform ItemParent()
        {
            return null;
        }

        protected virtual void OnAvatarSpeedChange(bool isSelfChange, float speed) { }
        protected virtual void InitialIKTargets(int itemSlotIndex, Transform IKDollNodes) { }

        protected virtual void RegisterAnimCallbacks() { }
        protected virtual void RegisterChatEventCallbacks(int slotIndex) { }
        protected virtual void RegisterAdditionalCameras() { }

        protected virtual void ExecuteExtraCmds() { }

        protected Transform FindPartMethod(string s) => ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, s);

        protected Transform LeftHand => ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "LeftHand");

        protected Transform RightHand => ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, "RightHand");
        protected void InstantiateItem(string name, string assetPath)
        {
            // instantiate
            // todo: collide avoid
            var cmd = new InstantiateObjectCmd(name, assetPath, Vector3.zero, Quaternion.identity, App.Stage.SpawnStrategy.DoNotOverride, _ItemProperties.relativeTransformNames);
            _StageUtils.ExecuteCmd(cmd);

            // register slots
            List<Transform> Slots = ArmatureUtils.FindChildrenByPartName(_Objects[name].transform, "Slot");
            if (Slots.Count > 0)
            {
                foreach (var slot in Slots)
                {
                    int i = int.Parse(slot.name.Substring("Slot".Length));
                    Debug.Log(string.Format("{0} item add slot {1}", name, i));
                    if (Slots.Count > 1)
                    {
                        ItemSlotTransformDictionary[i] = slot.transform;
                    }
                    else
                    {
                        // single avatar item
                        ItemSlotTransformDictionary[i] = null;
                    }
                }
            }

            // gen pos
            Vector3 posPosition = ItemPositionFromUser(AffectAvatarUser);
            Debug.Log("Instantiate item " + AffectAvatarUser.name + " " + posPosition.ToString());
            var count = 1;
            for(int i=0;i<OtherAvatarUsers.Count;i++ )
            {
                AvatarUser otherUser = OtherAvatarUsers[i];
                if (ItemSlotTransformDictionary[i] == null)
                {
                    continue;
                }
                posPosition += ItemPositionFromUser(otherUser);
                count++;
                Debug.Log("Instantiate item " + otherUser.name + " " + posPosition.ToString());
            }

            var itemPosition = posPosition / count;
            var itemRotation = ItemRotationFromUser(AffectAvatarUser);

            // reset item global transform
            _Objects[name].transform.position = itemPosition;
            _Objects[name].transform.rotation = itemRotation;

            // reset item position to relative transform
            for (int i = 0; i < _ItemProperties.relativeTransformNames.Count; i++)
            {
                Transform relativeTransform = _Objects[name].transform.Find("Relative_" + _ItemProperties.relativeTransformNames[i] + i);
                // ArmatureUtils.NormalizeRelative(relativeTransform, AffectAvatarUser.ActiveAvatarTransform);
                ResetItemTransform(_Objects[name].transform, relativeTransform, ArmatureUtils.FindPartString(AffectAvatarUser.ActiveAvatarTransform, _ItemProperties.relativeTransformNames[i]));
            }

            var mapPath = ItemManager.FindStageItem()._Objects["grass"].transform.Find("path");
            _Objects[name].transform.SetParent(mapPath);

            // reset parent
            if (ItemParent() != null)
            {
                _Objects[_ItemProperties.Name].transform.parent = ItemParent();
            }
        }

        private void SetAvatarPositions()
        {
            if (ItemSlotTransformDictionary.Count < OtherAvatarUsers.Count + 1)
            {
                Debug.Log("Item not instantiated any slots " + ItemProperties.Name);
                return;
            }

            _ActorsUtils.ExecuteCmd(new SetAvatarPositionSlotCmd(AffectAvatarUser, (int)_ItemProperties.EffectArea, 0, AffectAvatarUserIndex));
            for (int i = 0; i < OtherAvatarUsers.Count; i++)
            {
                _ActorsUtils.ExecuteCmd(new SetAvatarPositionSlotCmd(OtherAvatarUsers[i], (int)_ItemProperties.EffectArea, i + 1, OtherAvatarUserIndices[i]));
            }
        }

        private void ResetAvatarPositions()
        {
            if (ItemSlotTransformDictionary.Count < OtherAvatarUsers.Count + 1)
            {
                Debug.Log("Item not instantiated any slots " + ItemProperties.Name);
                return;
            }

            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                _ActorsUtils.ExecuteCmd(new RemoveAvatarPositionSlotCmd(ItemSlotUserDictionary[i].AvatarUser, i));
            }
        }

        private void SetProperties()
        {
            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                ItemSlotUserDictionary[i].AvatarUser.PropertiesPlanner.AddSpeedChangeListeners( ItemId, OnAvatarSpeedChange);
            }
            if (_ItemProperties.HasSpeed)
            {
                for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
                {
                    ItemSlotUserDictionary[i].AvatarUser.PropertiesPlanner.ChangeSpeed(_ItemProperties.Speed, ItemSlotUserDictionary[i].AvatarUser.AvatarUUID == AffectAvatarUser.AvatarUUID);
                }
            }
            else
            {
                OnAvatarSpeedChange(true, AffectAvatarUser.PropertiesPlanner.AvatarProperties.Speed);
            }
        }

        private void CleanProperties()
        {
            AffectAvatarUser.PropertiesPlanner.RemoveSpeedChangeListeners( ItemId, OnAvatarSpeedChange);
            foreach (var user in OtherAvatarUsers)
            {
                user.PropertiesPlanner.RemoveSpeedChangeListeners( ItemId, OnAvatarSpeedChange);
            }
            if (_ItemProperties.HasSpeed)
            {
                for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
                {
                    ItemSlotUserDictionary[i].AvatarUser.PropertiesPlanner.ChangeSpeed(0, ItemSlotUserDictionary[i].AvatarUser.AvatarUUID == AffectAvatarUser.AvatarUUID);
                }
            }
        }

        protected void RegisterInitialIKTargets()
        {
            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes"+i.ToString());
                if (IKDollNodes == null)
                {
                    continue;
                }
                InitialIKTargets(i, IKDollNodes);
                _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[i].AvatarUser, _ItemProperties.SlotNames[i], 
                    _ItemProperties.ikTargetsDictionary[i]));
            }
            
        }

        protected void RegisterIKDollNodesTrackingDict()
        {
            //comment use multiikmanager fakeBodyFollowDictionary
            if (_ItemProperties.IKDollNodesTrackingSubItemDict.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes" + i.ToString());
                if (IKDollNodes == null)
                {
                    continue;
                }

                foreach (var v in _ItemProperties.IKDollNodesTrackingSubItemDict)
                {
                    Transform p1 = IKDollNodes.Find("IKDollNodes" + v.Key);
                    Transform p2 = GameObject.Find(v.Value).GetComponent<Transform>();

                    Transform fakeP1;
                    fakeP1 = new GameObject(i.ToString() + "PairFake" + v.Key).transform;
                    fakeP1.parent = p1.parent;
                    fakeP1.localPosition = p1.localPosition;
                    fakeP1.localRotation = p1.localRotation;
                    fakeP1.transform.SetParent(p2);
                    SetTransformFollower(p1, fakeP1);
                }
            }
        }

        protected void RegisterPosers()
        {

            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes"+i.ToString());
                if (IKDollNodes == null)
                {
                    continue;
                }
                if (_ItemProperties.fakeOrFollows.Count > 0 && _ItemProperties.fakeOrFollows[i].FakeNames != null)
                {
                    IKGoalDict.Add(new Dictionary<string, GameObject>());
                    string[] ks = new string[_ItemProperties.fakeOrFollows[i].FakeNames.Keys.Count];
                    _ItemProperties.fakeOrFollows[i].FakeNames.Keys.CopyTo(ks, 0);
                    foreach (var fvk in ks)
                    {
                        Transform ikPoser = IKDollNodes.Find("IKDollNodes" + fvk);
                        if (ikPoser != null)
                        {
                            IKGoalDict[i].Add(fvk, Instantiate(ikPoser.gameObject));
                            IKGoalDict[i][fvk].name = "IKGoal" + fvk;
                            IKGoalDict[i][fvk].transform.position = ikPoser.position;
                            IKGoalDict[i][fvk].transform.rotation = ikPoser.rotation;

                            Transform fakePoser;
                            Transform peer = FindPartMethod(fvk);
                            fakePoser = new GameObject("Fake" + fvk).transform;
                            fakePoser.parent = peer.parent;
                            fakePoser.localPosition = peer.localPosition;
                            fakePoser.localRotation = peer.localRotation;
                            _Objects.Add("Fake" + fvk, fakePoser.gameObject);
                            _ItemProperties.fakeOrFollows[i].FakeNames[fvk] = fakePoser.gameObject;
                            SetTransformFollower(fakePoser.transform, peer.transform);
                            IKGoalDict[i][fvk].transform.parent =
                                ArmatureUtils.FindPartString(ItemSlotUserDictionary[i].AvatarUser.ActiveAvatarTransform, "Spine2");
                            _Objects.Add(fvk + "IKGoal", IKGoalDict[i][fvk]);
                        }
                    }

                    if (_ItemProperties.fakeOrFollows[i].FollowObject != "")
                    {
                        ResetItemTransform(_Objects[_ItemProperties.Name].transform, IKDollNodes.Find("IKDollNodes"+ _ItemProperties.fakeOrFollows[i].FollowObject), _ItemProperties.fakeOrFollows[i].FakeNames[_ItemProperties.fakeOrFollows[i].FollowObject].transform);
                    }
                }
            }
        }

        protected void SetDeformerOptions()
        {
            if (_ItemProperties.EulerAngleDeformerOptions != null)
            {
                AffectAvatarUser.MultiIKManager.Deformer.SetOptions(_ItemProperties.EulerAngleDeformerOptions);
            }
        }
        protected void ResetItemTransform(Transform Item, Transform objBelongtoItem, Transform objRelative)
        {
            objBelongtoItem.parent = objRelative;
            Item.parent = objBelongtoItem;
            objBelongtoItem.localPosition = Vector3.zero;
            objBelongtoItem.localRotation = Quaternion.identity;
            Item.parent = objRelative;
            objBelongtoItem.parent = Item;
        }

        protected void SetTransformFollower(Transform _follower, Transform _target)
        {
            AffectAvatarUser.MultiIKManager.AddFollowers(ItemId, _follower, _target);
        }

        protected void LoadSoundAsset()
        {
            if (_ItemProperties.SoundAssetPath != null)
            {
                var clip = Addressables.LoadAssetAsync<AudioClip>(
                    _ItemProperties.SoundAssetPath).WaitForCompletion();
                ServiceLocator.AudioService.AddAudioToMixerGroup(ItemId, 
                    _ItemProperties.Name, clip, Audio.AudioGroup.Effect, _ItemProperties.SoundCycle);
            }
        }

        protected void SetAvatarStatusAnimations()
        {
            //comment: fix substatus fade out upper body if action
            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                ItemSlotUserDictionary[i].AvatarUser.GestureBehaviorPlanner.OnMaskInit(Addressables.LoadAssetAsync<AvatarMask>
                        ("Assets/Models/Full_Body.mask").WaitForCompletion());
            }

            // Set IdleStatus anim
            // TODO: First idle equals second idle
            if (_ItemProperties.SecondIdleStatusMaskPath.Count > 0)
            {
                Debug.Assert(_ItemProperties.SecondIdleStatusMaskPath.Count == _ItemProperties.IdleStatusMaskPath.Count, string.Format("BaseItem {0} SetAvatarStatusAnimations fail second idle mask length invalid", _ItemProperties.Name));
            }

            if (_ItemProperties.IdleStatusAnimPath.Count > 0)
            {
                for(int i = 0; i < _ItemProperties.IdleStatusAnimPath.Count; i++)
                {
                    var idleAnim = Addressables.LoadAssetAsync<GameObject>
                        (_ItemProperties.IdleStatusAnimPath[i]).WaitForCompletion();
                    var idleMask = Addressables.LoadAssetAsync<AvatarMask>
                        (_ItemProperties.IdleStatusMaskPath[i]).WaitForCompletion();

                    _IdleStatus.Add(new ItemIdleStatusGroup());
                    _IdleStatus[i]._StatusAnimations = new ClipTransition[1];
                    _IdleStatus[i]._StatusAnimations[0] = idleAnim.GetComponent<ClipTransitionPrefab>().ClipTransition;
                    _IdleStatus[i]._AvatarMasks = new AvatarMask[1];
                    _IdleStatus[i]._AvatarMasks[0] = idleMask;

                    _SecondIdleStatus.Add(new ItemSecondIdleStatusGroup());
                    _SecondIdleStatus[i]._StatusAnimations = new ClipTransition[1];
                    _SecondIdleStatus[i]._StatusAnimations[0] = idleAnim.GetComponent<ClipTransitionPrefab>().ClipTransition;
                    _SecondIdleStatus[i]._AvatarMasks = new AvatarMask[1];
                    _SecondIdleStatus[i]._AvatarMasks[0] = idleMask;
                    if(_ItemProperties.SecondIdleStatusMaskPath.Count > i)
                    {

                        _SecondIdleStatus[i]._AvatarMasks[0] = Addressables.LoadAssetAsync<AvatarMask>(_ItemProperties.SecondIdleStatusMaskPath[i]).WaitForCompletion();
                    }

                    _ActorsUtils.ExecuteCmd(new SetAvatarIdleStatusCmd(ItemSlotUserDictionary[i].AvatarUser, _IdleStatus[i], _SecondIdleStatus[i]));
                }
            }

            // Set SubStatus anim
            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                if ( this is ClearItem )
                {
                    continue;
                }

                if (_ItemProperties.SubStatusAnimPath.Count <= i)
                {
                    _ActorsUtils.ExecuteCmd(new SetAvatarIdleSubStatusCmd(ItemSlotUserDictionary[i].AvatarUser, null, 0));
                    continue;
                }

                if (_ItemProperties.SubStatusAnimPath[i] == "")
                {
                    _ActorsUtils.ExecuteCmd(new SetAvatarIdleSubStatusCmd(ItemSlotUserDictionary[i].AvatarUser, null, 0));
                    continue;
                }

                if (_ItemProperties.SubStatusAnimPath[i] != "")
                {
                    var subAnim = Addressables.LoadAssetAsync<GameObject>
                    (_ItemProperties.SubStatusAnimPath[i]).WaitForCompletion();
                    var subMask = Addressables.LoadAssetAsync<AvatarMask>
                        (_ItemProperties.SubStatusMaskPath[i]).WaitForCompletion();

                    _SubStatus.Add(new ItemIdleSubStatusGroup());
                    _SubStatus[i]._StatusAnimations = new ClipTransition[1];
                    _SubStatus[i]._StatusAnimations[0] = subAnim.GetComponent<ClipTransitionPrefab>().ClipTransition;
                    _SubStatus[i]._AvatarMasks = new AvatarMask[1];
                    _SubStatus[i]._AvatarMasks[0] = subMask;
                    _ActorsUtils.ExecuteCmd(new SetAvatarIdleSubStatusCmd(ItemSlotUserDictionary[i].AvatarUser, _SubStatus[i], 0));
                }
            }

            // Set SilenceStatus anim
            for (int i = 0; i < _ItemProperties.SilenceStatusAnimPath.Count; i++)
            {
                if (_ItemProperties.SilenceStatusAnimPath[i] != "")
                {
                    var silenceAnim = Addressables.LoadAssetAsync<GameObject>
                    (_ItemProperties.SilenceStatusAnimPath[i]).WaitForCompletion();

                    var silenceMask = Addressables.LoadAssetAsync<AvatarMask>
                        (_ItemProperties.SilenceStatusMaskPath[i]).WaitForCompletion();

                    _SilenceStatus.Add(new ItemSilenceStatusGroup());
                    _SilenceStatus[i]._StatusAnimations = new ClipTransition[1];
                    _SilenceStatus[i]._StatusAnimations[0] = silenceAnim.GetComponent<ClipTransitionPrefab>().ClipTransition;
                    _SilenceStatus[i]._AvatarMasks = new AvatarMask[1];
                    _SilenceStatus[i]._AvatarMasks[0] = silenceMask;
                    _ActorsUtils.ExecuteCmd(new SetAvatarSilenceStatusCmd(ItemSlotUserDictionary[i].AvatarUser, _SilenceStatus[i]));
                }
            }

            // Set ActionStatus anim
            for (int i = 0; i < _ItemProperties.ActionStatusAnimPath.Count; i++)
            {
                if (_ItemProperties.ActionStatusAnimPath[i] != "")
                {
                    var actionAnim = Addressables.LoadAssetAsync<GameObject>
                    (_ItemProperties.ActionStatusAnimPath[i]).WaitForCompletion();

                    var actionMask = Addressables.LoadAssetAsync<AvatarMask>
                        (_ItemProperties.ActionStatusMaskPath[i]).WaitForCompletion();

                    _ActionStatus.Add(new ItemActionStatusGroup());
                    _ActionStatus[i]._StatusAnimations = new ClipTransition[1];
                    _ActionStatus[i]._StatusAnimations[0] = actionAnim.GetComponent<ClipTransitionPrefab>().ClipTransition;
                    _ActionStatus[i]._AvatarMasks = new AvatarMask[1];
                    _ActionStatus[i]._AvatarMasks[0] = actionMask;
                    _ActorsUtils.ExecuteCmd(new SetAvatarActionStatusCmd(ItemSlotUserDictionary[i].AvatarUser, _ActionStatus[i]));
                }
            }

            // Set InteractionStatus anim
            for (int i = 0; i < _ItemProperties.InteractionAnimPath.Count; i++)
            {
                if (_ItemProperties.InteractionAnimPath[i] != "")
                {
                    var intAnim = Addressables.LoadAssetAsync<GameObject>
                    (_ItemProperties.InteractionAnimPath[i]).WaitForCompletion();
                    var intMask = Addressables.LoadAssetAsync<AvatarMask>
                        (_ItemProperties.InteractionMaskPath[i]).WaitForCompletion();

                    _InteractionStatus.Add(new ItemInteractionStatusGroup());
                    _InteractionStatus[i]._StatusAnimations = new ClipTransition[1];
                    _InteractionStatus[i]._StatusAnimations[0] = intAnim.GetComponent<ClipTransitionPrefab>().ClipTransition;
                    _InteractionStatus[i]._AvatarMasks = new AvatarMask[1];
                    _InteractionStatus[i]._AvatarMasks[0] = intMask;
                }
            }

            RegisterAnimCallbacks();
        }

        protected void RemoveAvatarStatusAnimations()
        {
            //clear baseidle anim
            if (_ItemProperties.IdleStatusAnimPath.Count > 0)
            {
                for (int i = 0; i < _ItemProperties.IdleStatusAnimPath.Count; i++)
                {
                    _ActorsUtils.ExecuteCmd(new SetAvatarIdleStatusCmd(ItemSlotUserDictionary[i].AvatarUser, null, null));
                }
            }
            // clear SubStatus anims
            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                _ActorsUtils.ExecuteCmd(new SetAvatarIdleSubStatusCmd(ItemSlotUserDictionary[i].AvatarUser,null, -1));
            }
            // Clear SilenceStatus anim
            for (int i = 0; i < _ItemProperties.SilenceStatusAnimPath.Count; i++)
            {
                if (_ItemProperties.SilenceStatusAnimPath[i] != "")
                {
                    _ActorsUtils.ExecuteCmd(new SetAvatarSilenceStatusCmd(ItemSlotUserDictionary[i].AvatarUser, null));
                }
            }
            // Clear ActionStatus anim
            for (int i = 0; i < _ItemProperties.ActionStatusAnimPath.Count; i++)
            {
                if (_ItemProperties.ActionStatusAnimPath[i] != "")
                {
                    _ActorsUtils.ExecuteCmd(new SetAvatarActionStatusCmd(ItemSlotUserDictionary[i].AvatarUser, null));
                }
            }
        }
        // TODO: Eventually remove virtual of this method
        protected virtual void ExecuteCmds()
        {
            if (_ItemProperties.ObjectAssetPath != null)
            {
                InstantiateItem(_ItemProperties.Name, _ItemProperties.ObjectAssetPath);
                SetAvatarPositions();
                RegisterIKDollNodesTrackingDict();
                RegisterPosers();
                RegisterInitialIKTargets();
                SetDeformerOptions();
            }
            else
            {
                //comment: ik and poser needs position set first
                SetAvatarPositions();
                SetDeformerOptions();
            }
            Debug.Log("Camera switch we are here");
            _BaseApp._CameraSwitchManager.SetCameraSwitchManagerSlotUser(this);
            LoadSoundAsset();
            SetAvatarStatusAnimations();
            for (int i = 0; i < ItemSlotUserDictionary.Count; i++)
            {
                RegisterChatEventCallbacks(i);
            }
            ExecuteExtraCmds();
            RegisterAdditionalCameras();
        }

        override public void AddInteraction(string name, UnityEvent condition, UnityAction action)
        {
            var registry = new InteractionResistry();
            registry.condition = condition;
            registry.action = action;
            _Interactions[name] = registry;
            condition.AddListener(action);
        }

        override public void RemoveInteractionById(string name)
        {
            _Interactions[name].condition.RemoveListener(_Interactions[name].action);
        }

        protected void AddCameraRule(CameraTiming timing, int priority, string name, string path, Transform parent, Transform lookAt = null)
        {
            // TODO: This should not reference grass
            var mapPath = ItemManager.FindStageItem()._Objects["grass"].transform.Find("path");

            _CinemachineUtils.ExecuteCmd(new AddCameraRuleCmd(timing,
                priority, name, path, parent, lookAt));

            var cam = ItemManager.FindStageItem()._Objects["grass"].transform.Find(name);
            cam.localPosition += mapPath.localPosition;
            cam.SetParent(mapPath);
        }

        public override void PlayMovableAnimationBySpeed(string movableKey, string animKey, float normalizedTime, float toSet)
        {
            if (!ItemProperties.IsMovable)
            {
                return;
            }

            var animator = _Objects[movableKey].GetComponent<Animator>();
            if (normalizedTime != 0)
            {
                animator.Play(animKey, -1, normalizedTime);
            }
            else
            {
                animator.Play(animKey);
            }
            
            animator.speed = toSet;
        }
        
        protected void LockArmIK(int slotIndex, bool leftOrRight, bool lockHand, bool lockElbow, bool lockPoser, int priority)
        {
            Transform IKDollNodes = _Objects[_ItemProperties.Name].transform.Find("IKDollNodes" + slotIndex.ToString());
            var rightHand = IKGoalDict.Count<=0||!IKGoalDict[slotIndex].ContainsKey("RightHand") ? IKDollNodes.Find("IKDollNodesRightHand") : IKGoalDict[slotIndex]["RightHand"].transform;
            var leftHand = IKGoalDict.Count<=0 || !IKGoalDict[slotIndex].ContainsKey("LeftHand") ? IKDollNodes.Find("IKDollNodesLeftHand") : IKGoalDict[slotIndex]["LeftHand"].transform;
            int weightHand = lockHand ? 1 : 0;
            int weightElbow = lockElbow ? 1 : 0;
            int weightPoser = lockPoser ? 1 : 0;

            Transform hand = leftOrRight? leftHand : rightHand;
            string elbowName = leftOrRight ? "LeftElbow" : "RightElbow";
            Transform elbow = IKGoalDict.Count > 0 && IKGoalDict[slotIndex].ContainsKey(elbowName)?IKGoalDict[slotIndex][elbowName].transform : IKDollNodes.Find(leftOrRight ? "IKDollNodesLeftElbow" : "IKDollNodesRightElbow");
            IKEffectorName iKEffectorHand = leftOrRight?IKEffectorName.LeftHand : IKEffectorName.RightHand;
            IKEffectorName iKEffectorPoser = leftOrRight ? IKEffectorName.LeftHandPoser: IKEffectorName.RightHandPoser;
            IKEffectorName iKEffectorElbow = leftOrRight ? IKEffectorName.LeftElbow: IKEffectorName.RightElbow;

            _ItemProperties.ikTargetsDictionary[slotIndex] = new Dictionary<IKEffectorName, IKTarget>();
            _ItemProperties.ikTargetsDictionary[slotIndex].Add(iKEffectorHand, new IKTarget(lockHand?hand:null, weightHand, weightHand, priority));
            _ItemProperties.ikTargetsDictionary[slotIndex].Add(iKEffectorElbow, new IKTarget(lockElbow ? elbow : null, weightElbow, weightElbow, priority));
            _ItemProperties.ikTargetsDictionary[slotIndex].Add(iKEffectorPoser, new IKTarget(lockPoser ? hand:null, weightPoser, weightPoser, priority));

            _ActorsUtils.ExecuteCmd(new UpdateAvatarItemSlotCmd(ItemSlotUserDictionary[slotIndex].AvatarUser, _ItemProperties.SlotNames[slotIndex], _ItemProperties.ikTargetsDictionary[slotIndex]));
        }
    }

}