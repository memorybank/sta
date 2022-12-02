using UnityEngine;
using TMPro;
using System.Collections.Generic;

using Playa.Event;


namespace Playa.Avatars
{
    public abstract class AvatarBrain : MonoBehaviour
    {
        // Composition
        protected EventSequencer _EventSequencer;
        public EventSequencer EventSequencer => _EventSequencer;
        public AvatarBehavior Behavior { get; protected set; }

        // Parent pointer
        [SerializeField] private AvatarUser _AvatarUser;
        public AvatarUser AvatarUser => _AvatarUser;

        // Destination
        public GestureBehaviorPlanner GestureBehaviorPlanner => _AvatarUser.GestureBehaviorPlanner;
        public FacialBehaviorPlanner FacialBehaviorPlanner => _AvatarUser.FacialBehaviorPlanner;
        public BehaviorPlanner BehaviorPlanner => _AvatarUser.BehaviorPlanner;

        private void Awake()
        {
            Behavior = new AvatarBehavior();
            Behavior.GestureBehavior = new IdleGestureBehavior();

            GestureBehaviorPlanner.SetAvatarAnimancerSpeed(1.0f);
        }

        public float GetSpeed()
        {
            return GestureBehaviorPlanner.GetAvatarAnimancerSpeed();
        }

        protected virtual void Update()
        {
            // current do nothing
        }
    }

}