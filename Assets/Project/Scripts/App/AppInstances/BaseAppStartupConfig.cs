using UnityEngine;

using Playa.Avatars;
using UnityEngine.UI;
using Playa.Common;

namespace Playa.App
{
    public class BaseAppStartupConfig
    {
        public AvatarUser[] AvatarUsers;

        public Transform[] AvatarSpawn;

        public Transform[] LookAtTargets;
        public Transform[] IKLookAtProbes;

        //comment
        //Item UI Scroll? maybe
    }
}
