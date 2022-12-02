using System;


namespace Playa.Avatars
{
    public enum AvatarBehaviorStateType
    {
        InvalidBehavior = 0,
        IdleGestureBehavior = 1,
        PrepGestureBehavior = 2,
        MetronomicGestureBehavior = 3,
        RelaxGestureBehavior = 4,
        StrokeGestureBehavior = 5,
    }

    [Serializable]
    public class BehaviorLogEntry
    {
        public AvatarBehaviorStateType Behavior;
        public string Name;
        public double Timestamp;
        public int PlayCount;

        public BehaviorLogEntry(AvatarBehaviorStateType behavior, string name, double timestamp)
        {
            Behavior = behavior;
            Name = name;
            Timestamp = timestamp;
        }
    }

}