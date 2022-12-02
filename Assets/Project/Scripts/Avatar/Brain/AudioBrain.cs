using Recognissimo.Core;
using UnityEngine;
using Playa.Audio.ASR;
using Playa.Common;
using Animancer.FSM;
using RootMotion.FinalIK;
using Playa.Common.Utils;

using Playa.Audio.VAD;
using System.Collections.Generic;
using Playa.Event;

namespace Playa.Avatars
{
    public sealed class AudioBrain : AvatarBrain
    {
        [SerializeField] private VADDetector _VADDetector;
        [SerializeField] private MicrophoneLoudnessDetector _MicrophoneLoudnessDetector;
        [SerializeField] private GCSRDetector _GCSRDetector;

        public VADDetector VADDetector => _VADDetector;
        public MicrophoneLoudnessDetector MicrophoneLoudnessDetector => _MicrophoneLoudnessDetector;
        public GCSRDetector GCSRDetector => _GCSRDetector;

        public float positionWeightMarker = 0;
        public int positionWeightFootstep = 0;

        [SerializeField] private float _PIDTimeFactor = 25;
        private List<float> _PIDTargetValueList;
        private List<float> _PIDTargetDtList;

        [SerializeField] private int _TargetPIDControlSampleSize = 8;

        private float _LastPauseSpeed = 1.0f;

        private void Start()
        {
            _EventSequencer = GameObject.Find(AvatarUser.GetAudioCompName()).GetComponentInChildren<EventSequencer>();
            _VADDetector = GameObject.Find(AvatarUser.GetAudioCompName()).GetComponentInChildren<VADDetector>();
            _MicrophoneLoudnessDetector = GameObject.Find(AvatarUser.GetAudioCompName()).GetComponentInChildren<MicrophoneLoudnessDetector>();
            _GCSRDetector = GameObject.Find(AvatarUser.GetAudioCompName()).GetComponentInChildren<GCSRDetector>();

            _PIDTargetValueList = new List<float>();
            _PIDTargetDtList = new List<float>();

            _EventSequencer.voiceActivityEvent.AddListener(OnVoiceActivityReady);
            _EventSequencer.ideationalUnitEvent.AddListener(OnIdeationalUnitDetect);
            _EventSequencer.syntaxRootEvent.AddListener(OnPunctuatioDetect);
            _EventSequencer.keywordEvent.AddListener(OnKeywordDetect);
            _EventSequencer.sentimentEvent.AddListener(OnSentimentDetect);

            // _MicrophoneLoudnessDetector.VoiceLoudnessEvent.AddListener(OnLoudnessDetect);
        }

        protected override void Update()
        {
            base.Update();
            // for ik position, current do nothing

        }
        
        // TODO: this should be private
        public void OnVoiceActivityReady(VoiceActivityUnit voiceActivityUnit, int speakerUUID)
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }
            switch (voiceActivityUnit.ActivityType)
            {
                case VoiceActivityType.Inactive:
                {
                    if (GestureBehaviorPlanner.AvatarUser.AvatarActivityType == AvatarActivityType.IsPausing)
                    {
                            GestureBehaviorPlanner.MustSetTheIntentTPS(_LastPauseSpeed, false);
                    }
                    GestureBehaviorPlanner.AvatarUser.AvatarActivityType = AvatarActivityType.IsIdle;

                    Behavior.GestureBehavior = new IdleGestureBehavior();
                    GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic).AvatarLayeredAnimationManager.FadeOutUpperBodyWithCustomDuration(1.0f);
                    GestureBehaviorPlanner.AvatarLayeredAnimationManager.ResetSubStatusTimestamp();
                    ((AvatarActionState)AvatarUser.GetAvatarState(AvatarStateType.ActionIdle)).TryReEnterState();
                    FacialBehaviorPlanner.ResetFacialExpressionBlendShapeTargetWeights();
                    GestureBehaviorPlanner.SetAvatarAnimancerSpeed(1.0f);
                    break;
                }
                case VoiceActivityType.Active:
                {
                    if (GestureBehaviorPlanner.AvatarUser.AvatarActivityType == AvatarActivityType.IsPausing)
                    {
                        GestureBehaviorPlanner.MustSetTheIntentTPS(_LastPauseSpeed, false);
                    }
                    GestureBehaviorPlanner.AvatarUser.AvatarActivityType = AvatarActivityType.IsTalking;

                        positionWeightFootstep = -1;
                    if (GestureBehaviorPlanner.AvatarActionStateMachine.CurrentState.GetType() == typeof(ActionIdleState)
                            || GestureBehaviorPlanner.AvatarActionStateMachine.CurrentState.GetType() == typeof(SilenceState))
                    {
                        // comment: remove start talking
                        if ((Behavior.GestureBehavior is not IdleGestureBehavior))
                        {
                            Behavior.GestureBehavior = new IdleGestureBehavior();
                            GestureBehaviorPlanner.BackToIdle();
                        }
                        
                        GestureBehaviorPlanner.BackToIDUMonotronic();
                    }
                    break;
                }
                case VoiceActivityType.Punctuated:
                {
                    // Allow new stroke sequence
                    GestureBehaviorPlanner.IDUStrokeMatcher.ResetState();
                    var unit = new PunctuationUnit(false);
                    _LastPauseSpeed = base.GetSpeed();
                    GestureBehaviorPlanner.MustSetTheIntentTPS((float)_LastPauseSpeed/2, true);

                    OnVoicePunctuatioDetect(unit);
                    break;
                }
                case VoiceActivityType.Silence:
                {
                    Behavior.GestureBehavior = new SilenceGestureBehavior();
                    GestureBehaviorPlanner.ReEnterSilence();
                    
                    break;
                }
                default:
                {
                    Debug.Log("Error");
                    break;
                }
            }
        }

        private void OnIdeationalUnitDetect(IdeationalUnit ideationalUnit)
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }

            float tw = 0;
            float tt = 0;
            foreach (Phrase p in ideationalUnit.Phrases)
            {
                string s = p.Text;
                foreach(char c in p.Text)
                {
                    if (c.ToString() != "£¬" &&
                        c.ToString() != "¡£"&&
                        c.ToString() != "?" &&
                        c.ToString() != "!")
                    {
                        tw = tw + 1;
                    }
                }

                tt += p.Duration;
            }

            if (tt != 0 && tw != 0)
            {
                _PIDTargetValueList.Add(tw);
                _PIDTargetDtList.Add(tt);

                if (_PIDTargetValueList.Count >= _TargetPIDControlSampleSize)
                {
                    // do update TPS
                    float sumTW = 0.0f;
                    float sumTT = 0.0f;

                    for (int i = 0; i < _PIDTargetValueList.Count; i++)
                    {
                        sumTW += _PIDTargetValueList[i];
                        sumTT += _PIDTargetDtList[i];
                    }

                    GestureBehaviorPlanner.ApplyIntentTPS(sumTW / sumTT);

                    _PIDTargetValueList.Clear();
                    _PIDTargetDtList.Clear();
                }
            }
        }

        private bool CanPunctuationBehavior(PunctuationUnit punctuationUnit)
        {
            if (!AvatarUser.IsActive)
            {
                return false;
            }

            // state check
            if (GestureBehaviorPlanner.AvatarActionStateMachine.CurrentState.GetType() != typeof(IDUMetronomicState))
            {
                return false;
            }

            // behavior planner
            Behavior.GestureBehavior = new MetronomicGestureBehavior();
            ((MetronomicGestureBehavior)Behavior.GestureBehavior).NewSequence = punctuationUnit.IsSentenceFinished;
            if (!BehaviorPlanner.CanBehavior(AvatarBehaviorStateType.MetronomicGestureBehavior) && !punctuationUnit.IsSentenceFinished)
            {
                return false;
            }

            return true;
        }

        private void OnVoicePunctuatioDetect(PunctuationUnit punctuationUnit)
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }

            if (!CanPunctuationBehavior(punctuationUnit))
            {
                return;
            }

            positionWeightFootstep = 1;
        }

        private void OnPunctuatioDetect(PunctuationUnit punctuationUnit)
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }

            // Allow new stroke sequence
            GestureBehaviorPlanner.IDUStrokeMatcher.ResetState();

            if (!CanPunctuationBehavior(punctuationUnit))
            {
                return;
            }
            
            ((IDUMetronomicState)GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic)).TryReEnterStateLog(true);
            positionWeightFootstep = 1;
        }

        private void OnKeywordDetect(KeywordUnit keywordUnit)
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }

            Behavior.GestureBehavior = new StrokeGestureBehavior();
            ((StrokeGestureBehavior)Behavior.GestureBehavior).Keyword = keywordUnit.keyword;
            Behavior.FacialBehavior = new FacialExpressionBehavior();
            ((FacialExpressionBehavior)Behavior.FacialBehavior).Keyword = keywordUnit.keyword;
            ((AvatarActionState)GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.IDUStroke)).TryReEnterState();
            FacialBehaviorPlanner.MatchBehavior();
        }

        private void OnSentimentDetect(SentimentUnit sentimentUnit) 
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }

            // comment: for planning sentiment facial
            //FacialBehaviorPlanner.SetFacialExpressionBlendShapeTargetWeightByScore(sentimentUnit.Score);
        }

        private void OnLoudnessDetect(VoiceLoudnessUnit voiceLoudnessUnit)
        {
            if (!AvatarUser.IsActive)
            {
                return;
            }

            FacialBehaviorPlanner.UpdateVisemeMultiplier(voiceLoudnessUnit.Loudness/MicrophoneLoudnessDetector.MinLoudness);
        }
    }

}