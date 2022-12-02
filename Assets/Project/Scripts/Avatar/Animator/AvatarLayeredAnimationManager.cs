// Animancer // https://kybernetik.com.au/animancer // Copyright 2022 Kybernetik //

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value.

using Animancer;
using Animancer.FSM;
using Animancer.Units;
using Playa.Common;
using Playa.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Playa.App;

namespace Animancer
{
    internal static class DefaultFadeDuration
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() => AnimancerPlayable.DefaultFadeDuration = 0.25f;
    }
}

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public sealed class AvatarLayeredAnimationManager : MonoBehaviour
    {
        protected const int kBaseLayer = 0;
        protected const int kActionLayer = 1;
        protected const int kInteractionLayer = 2;

        /************************************************************************************************************************/

        [SerializeField]
        private AvatarAnimator _Avatar;
        [SerializeField] private AvatarMask _BaseMask;
        private AvatarMask _ActionMask;
        private AvatarMask _SubMask;
        private AvatarMask _InteractionMask;
        [SerializeField, Seconds] private float _ActionFadeDuration = AnimancerPlayable.DefaultFadeDuration;

        private bool _CanPlayActionFullBody;

        private double _NextSubStatusTimestamp = 0;

        [SerializeField] private ItemIdleSubStatusGroup _IdleSubStatusGroup;
        private OptionsStore _GroupOptions;

        private System.Random rnd = new System.Random();
        [SerializeField] private float _MinInterval = 5.0f;
        [SerializeField] private float _MaxInterval = 10.0f;

        public ItemIdleSubStatusGroup IdleSubStatusGroup => _IdleSubStatusGroup;

        /************************************************************************************************************************/

        private void ResetAvatar(AvatarAnimator toSet)
        {
            _Avatar = toSet;
        }

        /************************************************************************************************************************/

        private void Awake()
        {
            ResetSubStatusTimestamp();
            _Avatar.Animancer.Layers[kActionLayer].SetMask(_BaseMask);
            _GroupOptions = new OptionsStore(new List<OptionClass>(), "SubStatusGroup");
            _GroupOptions.AddOption(new SubStatusGroupOption(0, _IdleSubStatusGroup));
        }

        /************************************************************************************************************************/
        //SubStatus
        public void ResetSubStatusTimestamp()
        {
            _NextSubStatusTimestamp = TimeUtils.GetMSTimestamp() + Random.Range(_MinInterval, _MaxInterval) * 1000;
        }

        public void SetSubStatusAnim(ItemId id, ItemIdleSubStatusGroup idleStatusGroup, int priority)
        {
            if (priority >= 0)
            {
                _GroupOptions.AddOption(new SubStatusGroupOption(id, idleStatusGroup, priority));
            }
            else
            {
                _GroupOptions.RemoveOption(new SubStatusGroupOption(id, idleStatusGroup, priority));
            }
            
        }

        private void Update()
        {
            if (TimeUtils.GetMSTimestamp() > _NextSubStatusTimestamp)
            {
                ResetSubStatusTimestamp();
                SubStatusGroupOption ssgo = (SubStatusGroupOption)_GroupOptions.GetOption();
                if (ssgo.IdleStatusGroup != null)
                {
                    _IdleSubStatusGroup = ssgo.IdleStatusGroup;
                }
                else
                {
                    _IdleSubStatusGroup = new ItemIdleSubStatusGroup();
                    _IdleSubStatusGroup._StatusAnimations = new ClipTransition[0];
                    _IdleSubStatusGroup._AvatarMasks = new AvatarMask[0];
                }
                
                if (!IsAnyPlayingAction()&& _IdleSubStatusGroup._StatusAnimations.Length>0)
                {
                    var index = rnd.Next() % _IdleSubStatusGroup._StatusAnimations.Length;
                    var baseState = CurrentBaseLayerState();
                    if (baseState == null)
                    {
                        return;
                    }
                    SetSubLayerMask(_IdleSubStatusGroup._AvatarMasks[index]);
                    PlayAction(_IdleSubStatusGroup._StatusAnimations[index]);
                    var actionState = CurrentActionLayerState();
                    actionState.Events.OnEnd = () =>
                    {
                        FadeOutUpperBodyWithCustomDuration(UserStateTransitionConstants.ActionLayerTransitionFadeDuration);
                    };
                    ExitEvent.Register(actionState, () =>
                    {
                        SetSubLayerMask(null);
                        Debug.Log("Item Events subAction exited");
                    });
                }
            }
        }

        /************************************************************************************************************************/

        public void PlayBase(ITransition transition, bool canPlayActionFullBody, float fadeDuration, FadeMode fadeMode)
        {
            _CanPlayActionFullBody = canPlayActionFullBody;

            if (_CanPlayActionFullBody && _Avatar.Animancer.Layers[kActionLayer].TargetWeight > 0)
            {
                PlayActionFullBody(fadeDuration);
            }
            else
            {
                _Avatar.Animancer.Layers[kBaseLayer].Play(transition, fadeDuration, fadeMode);
            }
        }

        /************************************************************************************************************************/

        public void PlayAction(ITransition transition)
        {
            _Avatar.Animancer.Layers[kActionLayer].Play(transition);

            if (_CanPlayActionFullBody)
                PlayActionFullBody(transition.FadeDuration);
        }

        /************************************************************************************************************************/

        public void PlayAction(ITransition transition, float fadeDuration, FadeMode fadeMode)
        {
            _Avatar.Animancer.Layers[kActionLayer].Play(transition, fadeDuration, fadeMode);

            if (_CanPlayActionFullBody)
                PlayActionFullBody(fadeDuration);
        }

        public void PlayInteraction(ITransition transition)
        {
            var state = _Avatar.Animancer.Layers[kInteractionLayer].Play(transition);
            state.Events.OnEnd = () =>
            {
                _Avatar.Animancer.Layers[kInteractionLayer].StartFade(0, UserStateTransitionConstants.InteractionLayerTransitionFadeDuration);
            };
        }

        /************************************************************************************************************************/

        public void PlayInteraction(ITransition transition, float fadeDuration, FadeMode fadeMode)
        {
            var state = _Avatar.Animancer.Layers[kInteractionLayer].Play(transition, fadeDuration, fadeMode);
            state.Events.OnEnd = () =>
            {
                _Avatar.Animancer.Layers[kInteractionLayer].StartFade(0, UserStateTransitionConstants.InteractionLayerTransitionFadeDuration);
            };
        }

        /************************************************************************************************************************/

        private void PlayActionFullBody(float fadeDuration)
        {
            var upperBodyState = _Avatar.Animancer.Layers[kActionLayer].CurrentState;
            var fullBodyClone = _Avatar.Animancer.Layers[kBaseLayer].GetOrCreateState(upperBodyState, upperBodyState.Clip);
            _Avatar.Animancer.Layers[kBaseLayer].Play(fullBodyClone, fadeDuration);
            fullBodyClone.NormalizedTime = upperBodyState.NormalizedTime;
        }

        /************************************************************************************************************************/

        public void PlayFullBodyAction(float fadeDuration)
        {
            var fullBodyState = _Avatar.Animancer.Layers[kBaseLayer].CurrentState;
            Debug.Log(fullBodyState.Key);
            Debug.Log("action layer " + _Avatar.Animancer.Layers[kActionLayer]);
            var actionBodyClone = _Avatar.Animancer.Layers[kActionLayer].GetOrCreateState(fullBodyState, fullBodyState.Clip);
            Debug.Log("action layer find " + actionBodyClone.Key + " " + actionBodyClone.Events.GetName(0));
            _Avatar.Animancer.Layers[kActionLayer].Play(actionBodyClone, fadeDuration);
            actionBodyClone.NormalizedTime = fullBodyState.NormalizedTime;
        }

        /************************************************************************************************************************/

        public void FadeOutUpperBody()
        {
            _Avatar.Animancer.Layers[kActionLayer].StartFade(0, _ActionFadeDuration);
        }

        /************************************************************************************************************************/

        public void FadeOutUpperBodyWithCustomDuration(float fadeDuration)
        {
            _Avatar.Animancer.Layers[kActionLayer].StartFade(0, fadeDuration);
        }

        /************************************************************************************************************************/

        public void SetBaseLayerMask(AvatarMask mask)
        {
            _BaseMask = mask;
            MixActionMask();
            MixInteractionMask();
        }

        public void SetActionLayerMask(AvatarMask mask)
        {
            _ActionMask = mask;
            MixActionMask();
        }

        public void SetSubLayerMask(AvatarMask mask)
        {
            _SubMask = mask;
            MixActionMask();
        }

        public void SetInteractionLayerMask(AvatarMask mask)
        {
            _InteractionMask = mask;
            MixInteractionMask();
        }

        private void MixActionMask()
        {
            AvatarMask mask = new AvatarMask();

            foreach (AvatarMaskBodyPart value in Enum.GetValues(typeof(AvatarMaskBodyPart)))
            {
                if (value == AvatarMaskBodyPart.LastBodyPart) continue;
                bool baseMaskColor = _BaseMask.GetHumanoidBodyPartActive(value);
                bool actionMaskColor = (_ActionMask == null)? true : _ActionMask.GetHumanoidBodyPartActive(value);
                bool subMaskColor = (_SubMask == null) ? true : _SubMask.GetHumanoidBodyPartActive(value);
                mask.SetHumanoidBodyPartActive(value,
                baseMaskColor && actionMaskColor && subMaskColor);
            }

            _Avatar.Animancer.Layers[kActionLayer].SetMask(mask);
        }


        private void MixInteractionMask()
        {
            AvatarMask mask = new AvatarMask();

            foreach (AvatarMaskBodyPart value in Enum.GetValues(typeof(AvatarMaskBodyPart)))
            {
                if (value == AvatarMaskBodyPart.LastBodyPart) continue;
                bool baseMaskColor = _BaseMask.GetHumanoidBodyPartActive(value);
                bool interactionMaskColor = (_InteractionMask == null) ? true : _InteractionMask.GetHumanoidBodyPartActive(value);                
                mask.SetHumanoidBodyPartActive(value, baseMaskColor && interactionMaskColor);
            }

            _Avatar.Animancer.Layers[kInteractionLayer].SetMask(mask);
        }

        /************************************************************************************************************************/

        public AnimancerState CurrentActionLayerState()
        {
            return _Avatar.Animancer.Layers[kActionLayer].CurrentState;
        }

        /************************************************************************************************************************/

        public AnimancerState CurrentBaseLayerState()
        {
            return _Avatar.Animancer.Layers[kBaseLayer].CurrentState;
        }

        /************************************************************************************************************************/

        public bool IsPlayingAction(AnimationClip clip)
        {
            return _Avatar.Animancer.Layers[kActionLayer].IsPlayingClip(clip);
        }

        /************************************************************************************************************************/

        public bool IsPlayingBase(AnimationClip clip)
        {
            return _Avatar.Animancer.Layers[kBaseLayer].IsPlayingClip(clip);
        }

        /************************************************************************************************************************/
        public bool IsAnyPlayingAction()
        {
            return _Avatar.Animancer.Layers[kActionLayer].IsAnyStatePlaying();
        }

        /************************************************************************************************************************/

        public bool IsAnyPlayingBase()
        {
            return _Avatar.Animancer.Layers[kBaseLayer].IsAnyStatePlaying();
        }

        /************************************************************************************************************************/
    }

    public class SubStatusGroupOption : OptionClass
    {
        private ItemId _ItemId;
        private ItemIdleSubStatusGroup _IdleStatusGroup;
        public ItemId ItemId => _ItemId;
        public ItemIdleSubStatusGroup IdleStatusGroup => _IdleStatusGroup;
        public SubStatusGroupOption(ItemId itemid, ItemIdleSubStatusGroup idleStatusGroup, int priority=0)
        {
            _ItemId = itemid;
            _IdleStatusGroup = idleStatusGroup;
            _Priority = priority;
        }

        public override string ToString()
        {
            return string.Format("ActionIdleGroupOption({0},{1})", ItemId, _IdleStatusGroup);
        }

        public override bool Compare(OptionClass obj)
        {
            Debug.Assert(obj is SubStatusGroupOption, string.Format("Option Manager : try to compare {0} with {1}", this, obj));
            bool result = true;
            SubStatusGroupOption bigo = (SubStatusGroupOption)obj;
            if (_ItemId != bigo.ItemId)
            {
                result = false;
            }
            Debug.Log(string.Format("Option Compare : {0} {1}equals to {2}", ToString(), (result ? "" : "not "), bigo.ToString()));
            return result;
        }

        public override void OnUpdate()
        {
            //do nothing
        }

        public override void OnRemove()
        {
            //do nothing
        }
    }
}
