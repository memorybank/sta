using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    public abstract class ObjectIKInteraction : MonoBehaviour
    {
        [SerializeField] protected AvatarAnimator _Avatar;
        [SerializeField] protected Transform _Target;

        [SerializeField,Range(0,1)] protected float _IKPositionWeight;
        [SerializeField,Range(0,1)] protected float _IKRotationWeight;
        protected Vector3 _StartingPosition;
        protected Quaternion _StartingRotation;

        // Awake() is very similar as Start()
        private void Awake()
        {
            _StartingPosition = _Target.transform.position;
            _StartingRotation = _Target.transform.rotation;
        }

        public abstract void OnAnimatorIK(int layerIndex);
    }
}

