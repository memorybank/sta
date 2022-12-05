using Animancer;
using Animancer.FSM;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Playa.NLP;

namespace Playa.Avatars
{

    // This class controls animations of an particular avatar
    public class AvatarAnimator : MonoBehaviour
    {
        // SerializeField let us view it in the inspector, it it s a field
        // The public one is a property, uses => as the expression
        // SerializeField only shows fields, but not properties
        public AvatarAnimationConfig AnimationConfig;

        // Parent pointer
        [SerializeField] private AvatarUser _AvatarUser;
        public AvatarUser AvatarUser => _AvatarUser;

        // Internal states

        [SerializeField] private AnimancerComponent _Animancer;
        public AnimancerComponent Animancer => _Animancer;

        public int IdleStatusIndex = 0;
        public int SilenceStatusIndex = 0;
        public int ActionStatusIndex = 0;

        [SerializeField] private ActionIdleState _ActionIdle;
        [SerializeField] private IDUMetronomicState _IDUMetronomic;
        [SerializeField] private StrokeState _IDUStroke;
        [SerializeField] private SilenceState _Silence;
        [SerializeField] private BaseIdleState _BaseIdle;

        public AvatarBaseState BaseIdleState => _BaseIdle;

        public AvatarActionState ActionIdleState => _ActionIdle;
        public AvatarActionState IDUMetronomic => _IDUMetronomic;
        public AvatarActionState IDUStroke => _IDUStroke;
        public AvatarActionState SilenceState => _Silence;



        public StateMachine<AvatarActionState> ActionStateMachine { get; private set; }

        public StateMachine<AvatarBaseState> BaseStateMachine { get; private set; }


        private void Init()
        {
            AnimationConfig = new AvatarAnimationConfig();
            BaseStateMachine = new StateMachine<AvatarBaseState>(_BaseIdle);
            ActionStateMachine = new StateMachine<AvatarActionState>(_ActionIdle);
        }

        public void OnAvatarComponentChanged(AnimancerComponent animancerComponent)
        {
            if (_Animancer == null)
            {
                _Animancer = animancerComponent;
                Init();
            }
            else
            {
                _Animancer = animancerComponent;
                BaseStateMachine.CurrentState.TryReEnterState();
                ActionStateMachine.CurrentState.TryReEnterState();
            }
        }

#if UNITY_EDITOR
        private void AfterInspectorGUI()
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                UnityEditor.EditorGUILayout.ObjectField("Current State", ActionStateMachine.CurrentState, typeof(AvatarActionState), true);
                GUI.enabled = enabled;
            }
        }
#endif
    }

}
