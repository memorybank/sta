using UnityEngine;

namespace Playa.Avatars
{
    public class IKTarget
    {

        public Transform target;
        public float positionWeight;
        public float rotationWeight;
        public int priority;

        public IKTarget()
        {
        }

        public IKTarget(Transform target, float positionWeight = 0, float rotationWeight = 0, int priority = 0)
        {
            this.target = target;
            this.positionWeight = positionWeight;
            this.rotationWeight = rotationWeight;
            this.priority = priority;
        }
    }
}