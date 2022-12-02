using System;

namespace Playa.Avatars
{

    [Serializable]
    public class AvatarAnimationConfig
    {
        public bool Reenter = true;
        public float MinSpeed = 0.25f;
        public float MaxSpeed = 4.0f;
    }
}
