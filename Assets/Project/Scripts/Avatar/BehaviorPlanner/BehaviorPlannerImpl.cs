using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Playa.Avatars
{
    public class BehaviorPlannerImpl : BehaviorPlanner
    {
        public float AllowSwitchNormalizedPlayDepth = 0.48f;

        public override bool CanBehavior(AvatarBehaviorStateType behavior)
        {
            switch (behavior)
            {
                case AvatarBehaviorStateType.MetronomicGestureBehavior:
                    float normalizedLoopTimes = 0;
                    if (_BehaviorLogList.Count > 0)
                    {
                        normalizedLoopTimes = (float)_BehaviorLogList[_BehaviorLogList.Count - 1].PlayCount;
                    }

                    //if (AvatarUser.AnimancerCurrentNormalizedTime + normalizedLoopTimes >= AllowSwitchNormalizedPlayDepth)
                    //{
                    //    return true;
                    //}
                    //return false;
                    return true;
                default:
                    return true;
            }
        }

        public override void LogBehavior(AvatarBehaviorStateType behavior, string name, double timestamp, bool isPlayCountReset)
        {
            BehaviorLogEntry log = new BehaviorLogEntry(behavior, name, timestamp);
            log.PlayCount = 0;
            if (!isPlayCountReset && _BehaviorLogList.Count > 0)
            {
                log.PlayCount = _BehaviorLogList[_BehaviorLogList.Count - 1].PlayCount + 1;
            }

            if (_BehaviorLogList.Count >= 100)
            {
                _BehaviorLogList.RemoveRange(0, _BehaviorLogList.Count - 99);
            }
            _BehaviorLogList.Add(log);
        }
    }

}
