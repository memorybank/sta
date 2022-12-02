using System;
using System.Collections.Generic;
using Animancer.FSM;
using Animancer;
using Playa.Common;
using Playa.App;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.UI;
using Playa.Item;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public enum AvatarStateType
    {
        ActionIdle, 
        StartTalking,
        IDUMetronomic,
        IDUStroke,
        IDURelax,
        FinishTalking,
        Silence,
        BaseIdle
    }

    // This class represents a user that roleplays an avatar, which tracks all of its
    // recorded behavior.
    public class AvatarUser : MonoBehaviour
    {
        // TODO: refactor it to true uuid
        public int AvatarUUID;
        // Composition
        // TODO: dynamic create
        // Brain that aggregates detector signals
        [SerializeField] private AvatarBrain _AvatarBrain;
        // Behavior planners
        [SerializeField] private BehaviorPlanner _BehaviorPlanner;
        [SerializeField] private GestureBehaviorPlanner _GestureBehaviorPlanner;
        [SerializeField] private FacialBehaviorPlanner _FacialBehaviorPlanner;
        private PropertiesPlanner _PropertiesPlanner;
        private ItemFactory _ItemFactory;
        [SerializeField] private SlotManager _SlotManager;
        private MultiIKManager _MultiIKManager;
        private HeadIKManager _HeadIKManager;
        private OptionsStore _PositionsStore;
        private Transform _Path;
        // Animator controller that translate behavior description to physical animation
        [SerializeField] private AvatarAnimator _AvatarAnimator;

        [SerializeField] private AvatarProfile _Profile;

        [SerializeField] private int _AvatarPrefabIndex = 1;

        // TODO: Refactor since some of these are beyond a single avatar scope. 
        [SerializeField] private Dropdown _AvatarPrefabDropdown;
        private Dictionary<string, Transform> _AnimancerComponentDict;
        private string _ActiveAnimancerName;

        private AvatarActivityType _AvatarActivityType = AvatarActivityType.IsIdle;

        [SerializeField] private FullBodyBipedIK _FullBodyBipedIk;

        // Public
        // TODO: Merge all planners into BehaviorPlanner
        public BehaviorPlanner BehaviorPlanner => _BehaviorPlanner;
        public GestureBehaviorPlanner GestureBehaviorPlanner => _GestureBehaviorPlanner;
        public FacialBehaviorPlanner FacialBehaviorPlanner => _FacialBehaviorPlanner;
        public PropertiesPlanner PropertiesPlanner => _PropertiesPlanner;
        public SlotManager SlotManager => _SlotManager;
        public HeadIKManager HeadIKManager => _HeadIKManager;

        public MultiIKManager MultiIKManager => _MultiIKManager;
        public AvatarBrain AvatarBrain => _AvatarBrain;
        public GestureBehavior AvatarBrainGestureBehavior => _GestureBehaviorPlanner.AvatarBrain?.Behavior?.GestureBehavior;
        public float AnimancerCurrentNormalizedTime => _AvatarAnimator.Animancer.States.Current.NormalizedTime;

        public AvatarAnimator AvatarAnimator => _AvatarAnimator;

        public StateMachine<AvatarActionState> AvatarActionStateMachine => _AvatarAnimator.ActionStateMachine;

        public StateMachine<AvatarBaseState> AvatarBaseStateMachine => _AvatarAnimator.BaseStateMachine;

        public Toggle MetronomicLoopToggle;

        public string ActiveAnimancerName => _ActiveAnimancerName;
        public Dictionary<string, Transform> AnimancerComponentDict => _AnimancerComponentDict;

        public Transform ActiveAvatarTransform => _AnimancerComponentDict[_ActiveAnimancerName];

        public List<Dropdown.OptionData> AnimancerNames;

        public AvatarActivityType AvatarActivityType
        {
            get => _AvatarActivityType;
            set
            {
                _AvatarActivityType = value;
            }
        }

        public bool IsActive;

        private void Start()
        {
            Debug.Assert(AvatarUUID != 0, "must set Avatar UUID");
            EnableIKPass();

            InitAvatarPrefabDropdown();

            HeadIKInit();

            _PositionsStore = new OptionsStore(new List<OptionClass>(), "AvatarPosition");
            _PropertiesPlanner = new PropertiesPlanner();
        }

        public void EnableIKPass()
        {
            // Enable IK Pass for all states:
            AvatarAnimator.Animancer.Playable.ApplyAnimatorIK = true;
            // Enable Foot IK Pass for all states;
            AvatarAnimator.Animancer.Playable.ApplyFootIK = true;
        }

        public Transform GetAvatarPosition()
        {
            GameObject gameObject = GameObject.Find(GetPrefabsPositionObjName());
            return gameObject.transform;
        }

        public void SetPrefabPositionRotationToTarget(ItemId id, int priority, Vector3 posOffset, Quaternion angleOffset)
        {
            Debug.Log(string.Format("SetPrefabPositionRotationToTarget {0} {1} {2}", id, posOffset, priority));
            _PositionsStore.AddOption(new AvatarPositionOption(id, priority, posOffset, angleOffset));
            ResetPrefabPositionRotationToTarget();
        }

        public void SetPath(Transform path)
        {
            _Path = path;
        }

        public Transform GetPath()
        {
            return _Path;
        }
        
        public void GetPrefabPositionRotationToTarget(ref Vector3 position, ref Quaternion quaternion)
        {
            AvatarPositionOption apo = (AvatarPositionOption)_PositionsStore.GetOption();
            position = apo.PosOffset;
            quaternion = apo.RotationOffset;
        }

        public void ResetPrefabPositionRotationToTarget()
        {
            AvatarPositionOption apo = (AvatarPositionOption)_PositionsStore.GetOption();
            if (apo == null)
            {
                return;
            }
            Transform avatar = GetAvatarPosition();
            Debug.Log(string.Format("ResetAvatarPositionRotationToTarget {0}=>{1}, {2}=>{3}",
                avatar.position, _Path.position + _Path.localRotation * apo.PosOffset,
                avatar.rotation, _Path.localRotation * apo.RotationOffset));
            avatar.position = _Path.position + _Path.localRotation * apo.PosOffset;
            avatar.rotation = _Path.localRotation * apo.RotationOffset;
        }

        public void RemovePrefabPositionRotation(ItemId id)
        {
            Debug.Log(string.Format("RemovePrefabPositionRotation {0}", id));
            _PositionsStore.RemoveOption(new AvatarPositionOption(id));
            ResetPrefabPositionRotationToTarget();
        }

        private void InitAvatarPrefabDropdown()
        {
            _AnimancerComponentDict = new Dictionary<string, Transform>();
            AnimancerNames = new List<Dropdown.OptionData>();
            GameObject gameObject = GameObject.Find(GetPrefabsObjName());
            foreach (Transform child in gameObject.transform)
            {
                _AnimancerComponentDict.Add(child.name, child);
                AnimancerNames.Add(new Dropdown.OptionData(child.name));
            }

            _AvatarPrefabDropdown.options = AnimancerNames;
            _ActiveAnimancerName = AnimancerNames[0].text;

            _AvatarPrefabDropdown.value =
                _AvatarPrefabDropdown.options.FindIndex(option => option.text == "");

            //comment: must reset this time
            AvatarBlendShapeManager blendShapeManager = _AnimancerComponentDict[_ActiveAnimancerName].GetComponent(typeof(AvatarBlendShapeManager)) as AvatarBlendShapeManager;
            if (FacialBehaviorPlanner != null)
            {
                FacialBehaviorPlanner.ResetMorphTarget(blendShapeManager);
            }

            _AvatarPrefabDropdown.onValueChanged.AddListener(index =>
            {
                ClearUserAffectItems();
                var optionText = _AvatarPrefabDropdown.options[index].text;
                var selectedPrefab = _AnimancerComponentDict[optionText];
                AnimancerComponent animancer = selectedPrefab.GetComponent(typeof(NamedAnimancerComponent)) as AnimancerComponent;
                if (System.Object.ReferenceEquals(animancer, null))
                {
                    return;
                }
                _AnimancerComponentDict[_ActiveAnimancerName].gameObject.SetActive(false);
                selectedPrefab.gameObject.SetActive(true);
                _ActiveAnimancerName = optionText;
                OnAvatarPrefabChanged(animancer);

                if (IsActive && FacialBehaviorPlanner != null)
                {
                    AvatarBlendShapeManager blendShapeManager = selectedPrefab.GetComponent(typeof(AvatarBlendShapeManager)) as AvatarBlendShapeManager;
                    FacialBehaviorPlanner.ResetMorphTarget(blendShapeManager);
                }
            });
        }

        private void HeadIKInit()
        {
            _HeadIKManager = new HeadIKManager();
            _AvatarBrain.EventSequencer.voiceActivityEvent.AddListener((VoiceActivityUnit voiceActivityUnit, int speakerUUID) => {
                HeadIKManager.ExcuteHeadIK(voiceActivityUnit.ActivityType);
            });
        }

        private void ClearUserAffectItems()
        {
            //comment: clear items that affectavataruser is this
            if (_ItemFactory == null)
            {
                _ItemFactory = GameObject.Find("ItemFactory").GetComponent<ItemFactory>();
            }

            if (_ItemFactory == null)
            {
                // cant find
                return;
            }

            _ItemFactory.InitAvatarUserItem();
        }

        private string GetPrefabsObjName()
        {
            switch (_AvatarPrefabIndex)
            {
                case 1:
                    return "AvatarPrefabs";
                case 2:
                    return "AvatarPrefabs2";
                default:
                    return "AvatarPrefabs";
            }
        }

        private string GetPrefabsPositionObjName()
        {
            switch (_AvatarPrefabIndex)
            {
                case 1:
                    return "AvatarPrefabPositions";
                case 2:
                    return "AvatarPrefabPositions2";
                default:
                    return "AvatarPrefabPositions";
            }
        }

        public string GetAudioCompName()
        {
            switch (_AvatarPrefabIndex)
            {
                case 1:
                    return "Audio1";
                case 2:
                    return "Audio2";
                default:
                    return "Audio1";
            }
        }
        private void OnAvatarPrefabChanged(AnimancerComponent animancerComponent)
        {
            // Transition old avatar to own state
            _AvatarAnimator.OnAvatarComponentChanged(animancerComponent);
            EnableIKPass();
            Debug.Log("OnAvatarPrefabChange");

            ChangeIKManager();
        }

        public void ChangeIKManager()
        {
            // Multi-ik manager currently needs to be bound to the model
            var animancerComponent = AvatarAnimator.Animancer;
            var multiIKManager = animancerComponent.GetComponent<MultiIKManager>();
            if (multiIKManager == null)
            {
                multiIKManager = animancerComponent.gameObject.AddComponent<MultiIKManager>();
            }
            multiIKManager.Reset(this);
            _MultiIKManager = multiIKManager;

        }

        public void OnAvatarPrefabChangedInChatScene(string selectedAnimancer)
        {
            string optionText = selectedAnimancer;
            var selectedPrefab = _AnimancerComponentDict[optionText];
            AnimancerComponent animancer = selectedPrefab.GetComponent(typeof(NamedAnimancerComponent)) as AnimancerComponent;
            if (System.Object.ReferenceEquals(animancer, null))
            {
                return;
            }
            _ActiveAnimancerName = optionText;
            OnAvatarPrefabChanged(animancer);
        }

        public Action GetStateFunction(StateActionType type)
        {
            switch (type)
            {
                case StateActionType.BackToIDUMonotronic:
                    return GestureBehaviorPlanner.BackToIDUMonotronic;
                case StateActionType.ForceIdleState:
                    return GestureBehaviorPlanner.ForceIdleState;
                case StateActionType.ReEnterIDUMonotronic:
                    return GestureBehaviorPlanner.ReEnterIDUMonotronic;
                case StateActionType.ReEnterNextIDUMonotronic:
                    return GestureBehaviorPlanner.ReEnterNextIDUMonotronic;
                case StateActionType.ReEnterSilence:
                    return GestureBehaviorPlanner.ReEnterSilence;
                case StateActionType.BackToIdle:
                    return GestureBehaviorPlanner.BackToIdle;
            }
            return null;
        }

        public void SetBrainGestureBehavior(GestureBehavior gestureBehavior)
        {
            GestureBehaviorPlanner.SetBrainGestureBehavior(gestureBehavior);
        }

        public int MatchBrainBehavior(AvatarStateType type)
        {
            return GestureBehaviorPlanner.BehaviorPlannerMatchBrainBehavior(type);
        }

        public AvatarState GetAvatarState(AvatarStateType type)
        {
            switch (type)
            {
                case AvatarStateType.ActionIdle:
                    return _AvatarAnimator.ActionIdleState;
                case AvatarStateType.StartTalking:
                    return _AvatarAnimator.StartTalking;
                case AvatarStateType.IDUMetronomic:
                    return _AvatarAnimator.IDUMetronomic;
                //case AvatarStateType.IDURelax:
                    //return _AvatarAnimator.IDURelax;
                case AvatarStateType.IDUStroke:
                    return _AvatarAnimator.IDUStroke;
                case AvatarStateType.FinishTalking:
                    return _AvatarAnimator.FinishTalking;
                case AvatarStateType.Silence:
                    return _AvatarAnimator.SilenceState;
                case AvatarStateType.BaseIdle:
                    return _AvatarAnimator.BaseIdleState;
            }
            return null;
        }

        public void SetAvatarPrefab(int index)
        {
            _AvatarPrefabDropdown.value = index;
        }

        private void Update()
        {
            HeadIKManager.UpdateHeadIKOptions();
        }
    }


    public class AvatarPositionOption : OptionClass
    {
        ItemId _ItemId;
        Vector3 _PosOffset;
        Quaternion _AngleOffset;

        public AvatarPositionOption(ItemId itemId, int priority, Vector3 posOffset, Quaternion angleOffset)
        {
            _ItemId = itemId;
            _Priority = priority;
            _PosOffset = posOffset;
            _AngleOffset = angleOffset;
        }

        public override string ToString()
        {
            return string.Format("AvatarPosition({0},{1},{2},{3})",_ItemId, _Priority, _PosOffset, _AngleOffset);
        }

        public AvatarPositionOption(ItemId itemId)
        {
            _ItemId = itemId;
        }

        public ItemId ItemId => _ItemId;
        public Vector3 PosOffset => _PosOffset;
        public Quaternion RotationOffset => _AngleOffset;


        public override bool Compare(OptionClass optionClass)
        {
            Debug.Assert(optionClass is AvatarPositionOption, string.Format("Option Manager : try to compare {0} with {1}", this, optionClass));
            bool result = true;
            AvatarPositionOption apo = (AvatarPositionOption)optionClass;
            if (_ItemId != apo._ItemId) 
                result = false;
            Debug.Log(string.Format("Option Compare : {0} {1}equals to {2}", ToString(), (result ? "" : "not "), apo.ToString()));
            return result;
        }

        public override void OnRemove(){ }

        public override void OnUpdate(){ }
    }
}
