using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Playa.Common;
using Animancer;

namespace Playa.Animations
{

    // The generation process should not be affected by user intent, but
    // could potentially influenced by user profile.
    public abstract class AnimationSequenceGenerator : MonoBehaviour
    {
        public int CurrentIndex = 0;

        public virtual int TotalNetwork { get; private set; }

        public virtual string CurrentClipName { get; private set; }

        public bool IsInit;

        public abstract void Init();
        public abstract void PickStartPoint(int index);
        public abstract ClipTransition GenNextClip();

    }
}