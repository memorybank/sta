using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    public class ObjectLeftHandIKInteraction : ObjectIKInteraction
    {
        [SerializeField] private bool isHold;
        [SerializeField] protected AvatarIKGoal _Type;

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);  //camare2D.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit)) 
                {
                    if (hit.collider.gameObject != _Target.gameObject)
                    {
                        return;
                    }

                    if (isHold)
                    {
                       _Target.transform.position = _StartingPosition;
                       _Target.transform.rotation = _StartingRotation;
                        isHold = false;
                        return;
                    }

                    isHold = true;
                }
            }
        }

        public override void OnAnimatorIK(int layerIndex)
        {
            if (!isHold)
            {
                return;
            }

            Vector3 _AvatarPosition = _Avatar.Animancer.Animator.bodyPosition;
            Quaternion _AvatarRotation = _Avatar.Animancer.Animator.bodyRotation;
           _Target.transform.position = _AvatarPosition + new Vector3(0,(float)0.326,(float)-0.234);
           _Target.transform.rotation = _AvatarRotation;
            Vector3 _IKPosition =_Target.transform.position + new Vector3((float)+0.05, 0, (float)-0.01);
            _AvatarRotation.eulerAngles = new Vector3(_AvatarRotation.eulerAngles.x-15, _AvatarRotation.eulerAngles.y+90, _AvatarRotation.eulerAngles.z+90);

            UpdateLeftHandIK(_Avatar.Animancer.Animator, _IKPosition, _AvatarRotation);
        }

        private void UpdateLeftHandIK(Animator _animator, Vector3 position, Quaternion rotation)
        {
            AvatarIKGoal lhandGoal = _Type;
            _animator.SetIKPositionWeight(lhandGoal, _IKPositionWeight);
            _animator.SetIKRotationWeight(lhandGoal, _IKRotationWeight);
            _animator.SetIKPosition(lhandGoal, position);
            _animator.SetIKRotation(lhandGoal, rotation);
        }
    }
}

