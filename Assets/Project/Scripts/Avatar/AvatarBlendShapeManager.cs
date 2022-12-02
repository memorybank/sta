using System;
using UnityEngine;
using System.Linq;

using cfg.keywords;

namespace Playa.Avatars
{
    [Serializable]
    public class AvatarFacialExpressionBlendTarget
    {
        public int AngerBlendTarget;

        public int DisgustBlendTarget;

        public int FearBlendTarget;

        public int JoyBlendTarget;

        public int SadnessBlendTarget;

        public int SurpriseBlendTarget;
    }

    public class AvatarBlendShapeManager : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;

        public int[] visemeToBlendTargets = Enumerable.Range(0, OVRLipSync.VisemeCount).ToArray();

        public int laughterBlendTarget = OVRLipSync.VisemeCount;

        public AvatarFacialExpressionBlendTarget FacialExpressionBlendTarget;
        
    }
}