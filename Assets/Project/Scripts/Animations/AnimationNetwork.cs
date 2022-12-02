using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using cfg.gesture;

namespace Playa.Animations
{
    public class MetronomicSequenceUnit
    {
        public cfg.gesture.Type MetronomicType;
        public int NewIndexInSequence;

        public MetronomicSequenceUnit(cfg.gesture.Type metronomicType, int newIndexInSequence)
        {
            MetronomicType = metronomicType;
            NewIndexInSequence = newIndexInSequence;
        }
    }

    public abstract class AnimationNetwork
    {
        [SerializeField] protected AnimationRepository _Repository;

        [SerializeField] protected string _StartPoint;

        public AnimationRepository Repository => _Repository;

        public string StartPoint => _StartPoint;

        public abstract List<string> GetReachableClips(string clipName);

        public abstract List<cfg.gesture.Type> GetReachableClipTypes(cfg.gesture.Type clipType);
        public abstract MetronomicSequenceUnit GetNextClipType(int clipSequenceIndex);
    }
}