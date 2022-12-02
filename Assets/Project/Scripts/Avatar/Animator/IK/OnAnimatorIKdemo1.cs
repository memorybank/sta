using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    public class OnAnimatorIKdemo1 : MonoBehaviour
    {
        [SerializeField] private ObjectIKInteraction[] IKFaces;

        private void OnAnimatorIK(int layerIndex)
        {
            foreach (ObjectIKInteraction ik in IKFaces)
            {
                ik.OnAnimatorIK(layerIndex);
            }
        }
    }
}

