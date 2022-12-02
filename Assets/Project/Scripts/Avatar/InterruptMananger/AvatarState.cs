using Animancer.FSM;
using UnityEngine;

namespace Playa.InterruptMananger
{
    using Animancer.FSM;
    using UnityEngine;

    // This class is in the Animancer.Examples.StateMachines.InterruptManagement namespace.
    // It inherits from the Animancer.Examples.StateMachines.Characters.CharacterState class.
    // THey can both have the same name because they are in different namespaces.
    public sealed class AvatarState : Avatars.AvatarActionState
    {
        public enum Priority
        {
            Low,// Could specify "Low = 0," if we want to be explicit.
            Medium,// Medium = 1,
            High,// High = 2,
        }  

        [SerializeField] private Priority _Priority => Priority.Low;

        public override bool CanExitState
        {
            get
            {
                var nextState = (AvatarState)StateChange<Avatars.AvatarActionState>.NextState;
                return nextState._Priority >= _Priority;
            }
        }
    }
}
