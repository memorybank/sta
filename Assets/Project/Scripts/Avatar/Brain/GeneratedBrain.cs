using Recognissimo.Core;
using UnityEngine;
using Playa.Common;
using Animancer.FSM;

namespace Playa.Avatars
{
    public sealed class GeneratedBrain : AvatarBrain
    {
        float residual = 0.0f;

        private void Start()
        {
        }

        private void Update()
        {
            residual -= Time.deltaTime;
            if (residual < 1e-5)
            {
                var duration = Random.Range(1.0f, 5.0f);
                residual += duration;
                // TODO: Generate intent, then behavior
                ((AvatarActionState)GestureBehaviorPlanner.AvatarUser.GetAvatarState(AvatarStateType.IDUMetronomic)).TryReEnterState();
            }
        }
    }

}