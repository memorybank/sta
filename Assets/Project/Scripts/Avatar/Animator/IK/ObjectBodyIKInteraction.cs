using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    public class ObjectBodyIKInteraction : ObjectIKInteraction
    {
        [SerializeField] private bool isHold;

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

            Vector3 v = _Avatar.Animancer.Animator.bodyPosition;
            _Avatar.Animancer.Animator.bodyPosition = new Vector3(v.x, _Target.position.y+(float)0.55, _Target.position.z- (float)0.9);
            Vector3 LeftLegPosition = new Vector3(v.x + (float)0.02, _Target.position.y+(float)0.1, _Target.position.z - (float)1);
            _Avatar.Animancer.Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _IKPositionWeight);
            _Avatar.Animancer.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftLegPosition);

            Vector3 RightLegPosition = new Vector3(v.x - (float)0.02, _Target.position.y+(float)0.1, _Target.position.z - (float)1.25) ;
            _Avatar.Animancer.Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _IKPositionWeight);
            _Avatar.Animancer.Animator.SetIKPosition(AvatarIKGoal.RightFoot, RightLegPosition);
        }
    }
}

