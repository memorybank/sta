using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace Playa.Avatars
{
    public enum StateActionType
    {
        ForceIdleState,
        BackToIDUMonotronic,
        BackToIdle,
        ReEnterIDUMonotronic,
        ReEnterNextIDUMonotronic,
        ReEnterSilence,
    } 

    public abstract class BehaviorPlanner : MonoBehaviour
    {
        // Parent pointer
        [SerializeField] private AvatarUser _AvatarUser;
        public AvatarUser AvatarUser => _AvatarUser;

        protected List<BehaviorLogEntry> _BehaviorLogList;

        void Awake()
        {
            _BehaviorLogList = new List<BehaviorLogEntry>();
        }

        public abstract bool CanBehavior(AvatarBehaviorStateType behavior);

        public abstract void LogBehavior(AvatarBehaviorStateType behavior, string name, double timestamp, bool isPlayCountReset);

    }
}
