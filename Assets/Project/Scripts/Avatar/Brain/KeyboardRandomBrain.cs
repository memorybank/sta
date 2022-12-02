using Animancer.FSM;
using Playa.Common;
using UnityEngine;

namespace Playa.Avatars
{
    public sealed class KeyboardRandomBrain : AvatarBrain
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ((AvatarActionState)GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.ActionIdle)).TryEnterState();
                return;
            }

            for (KeyCode key = KeyCode.None; key < KeyCode.Joystick8Button19; key++)
            {
                if (Input.GetKey(key))
                {
                    // Update intent
                    Behavior.GestureBehavior =  new StrokeGestureBehavior();
                    ((StrokeGestureBehavior)Behavior.GestureBehavior).Keyword = key.ToString();

                    if (GestureBehaviorPlanner.AvatarUser.AvatarAnimator.AnimationConfig.Reenter)
                    {
                        GestureBehaviorPlanner.BackToIDUMonotronic();
                    }
                    else
                    {
                        GestureBehaviorPlanner.BackToIDUMonotronic();
                    }
                    break;
                }
            }
        }

    }

}