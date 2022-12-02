using UnityEngine;

using Playa.Common;

namespace Playa.Avatars
{
    [System.Serializable]
    public class AvatarBehavior
    {
        public GestureBehavior GestureBehavior;
        // Body behavior
        public FacialBehavior FacialBehavior;
    }

    public class GestureBehavior
    {
    }

    public class IdleGestureBehavior : GestureBehavior
    {
    }

    public class SilenceGestureBehavior : GestureBehavior
    {
    }

    public class PrepGestureBehavior : GestureBehavior
    {
    }

    public class MetronomicGestureBehavior : GestureBehavior
    {
        public bool NewSequence;
    }

    public class RelaxGestureBehavior : GestureBehavior
    {
    }

    public class StrokeGestureBehavior : GestureBehavior
    {
        public string Keyword;
    }

    public class FacialBehavior
    { 
    }

    public class FacialExpressionBehavior : FacialBehavior
    {
        // equals StrokeGestureBehavior.Keyword
        public string Keyword;
    }

}