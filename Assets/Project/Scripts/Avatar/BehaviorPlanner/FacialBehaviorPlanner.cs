using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

using cfg.keywords;
using Playa.Common;

namespace Playa.Avatars
{
    public class FacialBehaviorPlanner : MonoBehaviour
    {
        // Internal states
        [SerializeField] private OVRLipSyncContextMorphTarget OVRLipSyncContextMorphTarget;
        [SerializeField] private float MaxVisemeMultiplier = 1.5f;
        [SerializeField] private float VisemeMultiplier;
        private AvatarFacialExpressionBlendTarget FacialExpressionBlendTarget;
        [SerializeField] private FacialKeywordMatcher _KeywordMatcher;
        private bool _IsRunning;
        private ConcurrentDictionary<int, float> _FacialExpressionPlanningDictionary;
        private ConcurrentDictionary<int, float> _FacialExpressionRenderingDictionary;
        [SerializeField] private float _FacialExpressionFadeInSpeed = 12.0f;
        [SerializeField] private float _FacialExpressionFadeOutSpeed = 1.0f;
        [SerializeField] private float _MinFacialVisemeMultiplier = 0.3f;

        // Parent Pointer
        [SerializeField] private AvatarUser _AvatarUser;

        // Source
        public AvatarBrain AvatarBrain => _AvatarUser.AvatarBrain;

        void Awake()
        {
            VisemeMultiplier = MaxVisemeMultiplier;

            FacialExpressionBlendTarget = new AvatarFacialExpressionBlendTarget();
            FacialExpressionBlendTarget.AngerBlendTarget = -1;
            FacialExpressionBlendTarget.DisgustBlendTarget = -1;
            FacialExpressionBlendTarget.FearBlendTarget = -1;
            FacialExpressionBlendTarget.JoyBlendTarget = -1;
            FacialExpressionBlendTarget.SadnessBlendTarget = -1;
            FacialExpressionBlendTarget.SurpriseBlendTarget = -1;

            _FacialExpressionPlanningDictionary = new ConcurrentDictionary<int, float>();
            _FacialExpressionRenderingDictionary = new ConcurrentDictionary<int, float>();
            Task.Run(() => TimerCoroutine());
        }

        void Update()
        {
            if (!AvatarBrain.AvatarUser.IsActive)
            {
                return;
            }

            if (OVRLipSyncContextMorphTarget.skinnedMeshRenderer == null)
            {
                return;
            }

            foreach (var f in _FacialExpressionRenderingDictionary.ToArray())
            {
                float lastValue = OVRLipSyncContextMorphTarget.skinnedMeshRenderer.GetBlendShapeWeight(f.Key);
                if (f.Value > 0.0f)
                {
                    if (lastValue < _FacialExpressionRenderingDictionary[f.Key])
                    {
                        float newValue = Mathf.Min(f.Value, lastValue + _FacialExpressionFadeInSpeed);
                        OVRLipSyncContextMorphTarget.skinnedMeshRenderer.SetBlendShapeWeight(f.Key, Mathf.Min(newValue, 100.0f));
                        _FacialExpressionRenderingDictionary.AddOrUpdate(f.Key, 0, (key, oldValue) => 0);

                        float multiplierSuggest = (float)((100 - newValue) / 100 * MaxVisemeMultiplier);
                        if (VisemeMultiplier > multiplierSuggest)
                        {
                            VisemeMultiplier = Mathf.Max(_MinFacialVisemeMultiplier, multiplierSuggest);
                        }
                    }
                }
                else if (lastValue > _FacialExpressionRenderingDictionary[f.Key])
                {
                    float newValue = Mathf.Max(0, lastValue - _FacialExpressionFadeOutSpeed);
                    OVRLipSyncContextMorphTarget.skinnedMeshRenderer.SetBlendShapeWeight(f.Key, Mathf.Min(newValue, 100.0f));
                }
            }
        }

        private void OnDestroy()
        {
            _FacialExpressionPlanningDictionary.Clear();
            _IsRunning = false;
        }

        public async Task TimerCoroutine()
        {
            _IsRunning = true;
            while (_IsRunning)
            {
                await Task.Delay((int)(ThreadsConstants.FacialExpressionPlanningCoroutineIntervalTime * 1000));
                await FacialExpressionRender();
            }
        }

        public async Task FacialExpressionRender()
        {
            foreach (var f in _FacialExpressionPlanningDictionary.ToArray())
            {
                _FacialExpressionRenderingDictionary.AddOrUpdate(f.Key, f.Value, (key, oldValue) => f.Value);
            }
        }

        public void ResetMorphTarget(AvatarBlendShapeManager m)
        {
            if (FacialExpressionBlendTarget != null)
            {
                ResetFacialExpressionBlendShapeTargetWeights();
            }
            _FacialExpressionPlanningDictionary.Clear();
            _FacialExpressionRenderingDictionary.Clear();

            if (m == null)
            {
                OVRLipSyncContextMorphTarget.skinnedMeshRenderer = null;
                OVRLipSyncContextMorphTarget.visemeToBlendTargets = null;
                OVRLipSyncContextMorphTarget.laughterBlendTarget = -1;

                FacialExpressionBlendTarget = new AvatarFacialExpressionBlendTarget();
                FacialExpressionBlendTarget.AngerBlendTarget = -1;
                FacialExpressionBlendTarget.DisgustBlendTarget = -1;
                FacialExpressionBlendTarget.FearBlendTarget = -1;
                FacialExpressionBlendTarget.JoyBlendTarget = -1;
                FacialExpressionBlendTarget.SadnessBlendTarget = -1;
                FacialExpressionBlendTarget.SurpriseBlendTarget = -1;
            }
            else
            {
                OVRLipSyncContextMorphTarget.skinnedMeshRenderer = m.skinnedMeshRenderer;
                OVRLipSyncContextMorphTarget.visemeToBlendTargets = m.visemeToBlendTargets;
                OVRLipSyncContextMorphTarget.laughterBlendTarget = m.laughterBlendTarget;
                FacialExpressionBlendTarget = m.FacialExpressionBlendTarget;
            }
        }

        public void UpdateVisemeMultiplier(float toSet)
        {
            if (toSet <= 0)
            {
                Debug.LogWarning("FacialBehaviorPlanner.UpdateVisemeMultiplier invalid "+toSet.ToString());
                return;
            }

            OVRLipSyncContextMorphTarget.VisemeMultiplier = Mathf.Min(toSet, VisemeMultiplier);
        }

        public float GetVisemeMultiplier()
        {
            return OVRLipSyncContextMorphTarget.VisemeMultiplier;
        }

        // Todo Other Facial Behavior Type
        // return if match
        public bool MatchBehavior()
        {
            if (FacialExpressionBlendTarget == null)
            {
                return false;
            }
            MatchResult matchResult = _KeywordMatcher.match(AvatarBrain.Behavior);
            if (matchResult != null && matchResult.FacialExpression.Count > 0)
            {
                ResetFacialExpressionBlendShapeTargetWeights();
                for (int i = 0; i < matchResult.FacialExpression.Count; i++)
                {
                    SetFacialExpressionBlendShapeTargetWeight(matchResult.FacialExpression[i], (float)matchResult.FacialExpressionAmplitude[i]);
                }

                return true;
            }
            else
            {
                ResetFacialExpressionBlendShapeTargetWeights();
            }

            return false;
        }

        public void SetFacialExpressionBlendShapeTargetWeightByScore(float score)
        {
            //comment: Ð§¼Û
            if (Mathf.Abs(score) <= 0.5)
            {
                return;
            }
            else
            {
                ResetFacialExpressionBlendShapeTargetWeights();
                if (score > 0)
                {
                    Debug.Log("facial Set Joy " + FacialExpressionBlendTarget.JoyBlendTarget);
                    SetFacialExpressionBlendShapeTargetWeight(FacialExpressionType.JOY, 100);
                }
                else
                {
                    Debug.Log("facial Set Sadness" + FacialExpressionBlendTarget.SadnessBlendTarget);
                    SetFacialExpressionBlendShapeTargetWeight(FacialExpressionType.SADNESS, 100);
                }

            }
        }

        public void ResetFacialExpressionBlendShapeTargetWeights()
        {
            VisemeMultiplier = MaxVisemeMultiplier;
            _FacialExpressionPlanningDictionary.AddOrUpdate(FacialExpressionBlendTarget.AngerBlendTarget, 0, (key, oldValue) => 0);
            _FacialExpressionPlanningDictionary.AddOrUpdate(FacialExpressionBlendTarget.DisgustBlendTarget, 0, (key, oldValue) => 0);
            _FacialExpressionPlanningDictionary.AddOrUpdate(FacialExpressionBlendTarget.FearBlendTarget, 0, (key, oldValue) => 0);
            _FacialExpressionPlanningDictionary.AddOrUpdate(FacialExpressionBlendTarget.JoyBlendTarget, 0, (key, oldValue) => 0);
            _FacialExpressionPlanningDictionary.AddOrUpdate(FacialExpressionBlendTarget.SadnessBlendTarget, 0, (key, oldValue) => 0);
            _FacialExpressionPlanningDictionary.AddOrUpdate(FacialExpressionBlendTarget.SurpriseBlendTarget, 0, (key, oldValue) => 0);
        }

        public void SetFacialExpressionBlendShapeTargetWeight(FacialExpressionType targetType, float toSet)
        {
            switch (targetType)
            {
                case FacialExpressionType.ANGER:
                    SetSkinMeshRendererBlendShapeWeightIfValid(FacialExpressionBlendTarget.AngerBlendTarget, toSet);
                    break;
                case FacialExpressionType.DISGUST:
                    SetSkinMeshRendererBlendShapeWeightIfValid(FacialExpressionBlendTarget.DisgustBlendTarget, toSet);
                    break;
                case FacialExpressionType.FEAR:
                    SetSkinMeshRendererBlendShapeWeightIfValid(FacialExpressionBlendTarget.FearBlendTarget, toSet);
                    break;
                case FacialExpressionType.JOY:
                    SetSkinMeshRendererBlendShapeWeightIfValid(FacialExpressionBlendTarget.JoyBlendTarget, toSet);
                    break;
                case FacialExpressionType.SADNESS:
                    SetSkinMeshRendererBlendShapeWeightIfValid(FacialExpressionBlendTarget.SadnessBlendTarget, toSet);
                    break;
                case FacialExpressionType.SURPRISE:
                    SetSkinMeshRendererBlendShapeWeightIfValid(FacialExpressionBlendTarget.SurpriseBlendTarget, toSet);
                    break;
                default:
                    return;
            }
        }

        private void SetSkinMeshRendererBlendShapeWeightIfValid(int index, float toSet)
        {
            if (index >= 0)
            {
                _FacialExpressionPlanningDictionary.AddOrUpdate(index, toSet, (key, oldValue) => Mathf.Max(oldValue, toSet));
            }
        }
    }
}

