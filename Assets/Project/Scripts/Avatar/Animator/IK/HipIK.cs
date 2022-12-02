using RootMotion.FinalIK;
using UnityEngine;

namespace Playa.Avatars.IK
{
    public class HipIK : MonoBehaviour
    {
        [SerializeField] private VRIK _VRIK;
        public Transform _Person;
        public Transform _SitPosition;

        private void Start()
        {
            IKPropertySetting();
        }

        void IKPropertySetting()
        {
            VRIK.References.AutoDetectReferences(_Person, out _VRIK.references);

            _VRIK.solver.spine.pelvisTarget = _SitPosition;
        }
    }
}

