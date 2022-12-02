using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class HoldSphere : MonoBehaviour
{
    public FullBodyBipedIK ik;
    public Transform leftHandTarget, rightHandTarget;

    public float positionTimer = 0;
    public bool isReverse;
    // Update is called once per frame
    void LateUpdate()
    {
        ik.solver.leftHandEffector.position = leftHandTarget.position;
        ik.solver.leftHandEffector.rotation = leftHandTarget.rotation;
        ik.solver.rightHandEffector.position = rightHandTarget.position;
        ik.solver.rightHandEffector.rotation = rightHandTarget.rotation;
        ik.solver.leftHandEffector.positionWeight = positionTimer*0.01f;
        ik.solver.rightHandEffector.positionWeight = positionTimer * 0.01f;
        ik.solver.leftHandEffector.rotationWeight = positionTimer * 0.01f;
        ik.solver.rightHandEffector.rotationWeight = positionTimer * 0.01f;

        float times = 1f;
        if (isReverse)
        {
            positionTimer -= times;
        }
        else
        {
            positionTimer += times;
        }
        
        if (positionTimer > 100)
        {
            isReverse = true;
        }
        if (positionTimer < 0)
        {
            positionTimer = 0;
            isReverse = false;
        }
    }
}
