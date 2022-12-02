using Playa.Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Animations
{
    [System.Serializable]

    public class MotionMatcher
    {
        public static void MatchByRefernce(Animator animator, List<ReferenceInfo> features, Transform reference)
        {
            var dataPoints = features.Count;
            var score = 0f;
            var index = -1;
            for (int i = 0; i < dataPoints; i++)
            {
                var curScore = Score(features[i], reference);
                if (score < curScore)
                {
                    score = curScore;
                    index = i;
                }
            }
            var delta = animator.GetCurrentAnimatorStateInfo(0).length * (1.0f * index / dataPoints -
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            if (delta < 1e-6)
            {
                delta += animator.GetCurrentAnimatorStateInfo(0).length;
            }

            animator.Update(delta);
        }

        public static float Score(ReferenceInfo info, Transform reference)
        {
            return MathUtils.Sigmoid(-(info.position - reference.position).magnitude);
        }
    }
}

