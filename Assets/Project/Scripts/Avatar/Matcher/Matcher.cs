using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa.Common;

using cfg.keywords;

namespace Playa.Avatars
{
    public class MatchResult
    {
        public int AnimationClipIndex;
        public List<FacialExpressionType> FacialExpression;
        public List<int> FacialExpressionAmplitude;

        public MatchResult()
        {
            AnimationClipIndex = -1;
            FacialExpression = new List<FacialExpressionType>();
            FacialExpressionAmplitude = new List<int>();
        }
    }

    public abstract class Matcher<T> : MonoBehaviour
    {
        public T DataSource;

        public Matcher(T source)
        {
            DataSource = source;
        }
        public virtual MatchResult match(AvatarBehavior behavior) { return new MatchResult(); }

        public virtual void ResetState() { }
    }
}