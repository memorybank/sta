using Playa.Common;
using System;
using UnityEngine.Events;
using UnityEngine;
using Animancer;

namespace Playa.Common.Anim.Event
{
    public class MetronomicAnimSequenceUnit
    {
        public bool IsNewSequence;
        public int Index;
        //AnimationClip Clip;

        public MetronomicAnimSequenceUnit(bool isNewSequence, int index)
        {
            IsNewSequence = isNewSequence;
            Index = index;
            //Clip = clip;
        }
    }

    public class MetronomicAnimClipUnit
    {
        public bool IsNext;
        public ClipTransition Clip;
        //AnimationClip Clip;

        public MetronomicAnimClipUnit(bool isNext, ClipTransition clip)
        {
            IsNext = isNext;
            Clip = clip;
        }
    }

    [Serializable]
    public class MetronomicAnimSequenceEvent : UnityEvent<MetronomicAnimSequenceUnit>
    {
    }

    [Serializable]
    public class MetronomicAnimClipEvent : UnityEvent<MetronomicAnimClipUnit>
    {
    }

}