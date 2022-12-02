using Playa.App;
using RootMotion.FinalIK;
using UnityEngine;

namespace Playa.Avatars.IK
{
    //comment : refactoring
    public class HeadIK:MonoBehaviour
    {
        public GameObject[] headList;
        public GameObject[] lookAtObject;
        public LookAtIK[] lookAt;

        public float _DefaultBodyWeight = 0.5f;
        public float _DefaultHeadWeight = 1.0f;

        private float speed = 2.0f;

        public void Init(StandAppStartupConfig config)
        {
            headList = new GameObject[2];
            lookAtObject = new GameObject[2];
            lookAtObject[0] = config.IKLookAtProbes[0].gameObject;
            lookAtObject[1] = config.IKLookAtProbes[1].gameObject;
            lookAt = new LookAtIK[2];
            lookAt[0] = config.AvatarUsers[0].AnimancerComponentDict[config.AvatarUsers[0].ActiveAnimancerName].GetComponent<LookAtIK>();
            lookAt[1] = config.AvatarUsers[1].AnimancerComponentDict[config.AvatarUsers[1].ActiveAnimancerName].GetComponent<LookAtIK>();
        }

        void LateUpdate()
        {
            if (lookAtObject.Length <= 1)
            {
                return;
            }
            for (int index = 0; index < 2; index++)
            {
                lookAtObject[index].transform.position +=
                (headList[index].transform.position - lookAtObject[index].transform.position) * Time.deltaTime * speed;
                lookAtObject[1 - index].transform.position +=
                (headList[index].transform.position - lookAtObject[1 - index].transform.position) * Time.deltaTime * speed;
            }
        }

        public void IKPropertySetting(int IKIndex)
        {
            if (IKIndex >= headList.Length)
            {
                Debug.LogWarning("head ik set head error out of range" + IKIndex);
                return;
            }

            // The master weight
            lookAt[IKIndex].solver.IKPositionWeight = 1f;
            // Changing the weights of individual body parts
            lookAt[IKIndex].solver.bodyWeight = _DefaultBodyWeight;
            lookAt[IKIndex].solver.headWeight = _DefaultHeadWeight;
            lookAt[IKIndex].solver.eyesWeight = 0.5f;
        }

        public void IKLookAtPositionSetting(int IKIndex, GameObject head)
        {
            if (IKIndex >= headList.Length)
            {
                Debug.LogWarning("head ik set head error out of range" + IKIndex);
                return;
            }
            headList[IKIndex] = head;
        }
    }
}

