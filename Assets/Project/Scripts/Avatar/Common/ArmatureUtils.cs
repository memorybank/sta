using Playa.Common;
using RootMotion;
using System.Collections.Generic;
using UnityEngine;
using finalIK = RootMotion.FinalIK;


namespace Playa.Avatars
{
    public class ArmatureUtils
    {
        private static object finalIK;

        // Amy arm length
        private static float _AmyArmLength = 0.3847948f;

        static public Transform FindLeftHand(Transform person)
        {
            return FindPartString(person, "LeftHand");
        }
        static public Transform FindRightHand(Transform person)
        {
            return FindPartString(person, "RightHand");
        }
        static public Transform FindHead(Transform person)
        {
            return FindPartString(person, "Head");
        }

        static public Transform FindPelvis(Transform person)
        {
            return FindPartString(person, "Hips");
        }

        static public Transform FindPartString(Transform person, string partName)
        {
            var fullBodyBiped = person.gameObject.GetComponent<finalIK.FullBodyBipedIK>();

            var references = new BipedReferences();
            if (fullBodyBiped != null)
            {
                references = fullBodyBiped.references;
            }
            else
            {
                BipedReferences.AutoDetectReferences(ref references, person, new BipedReferences.AutoDetectParams(true, false));
            }

            switch (partName)
            {
                case "Root":
                    return references.root;
                case "Hips":
                    return references.pelvis;
                case "Spine":
                    return references.spine[0];
                case "Spine1":
                    return references.spine[1];
                case "Spine2":
                    return references.spine[1];
                case "Head":
                    return references.head;
                case "LeftShoulder":
                    return references.leftUpperArm.parent;
                case "LeftArm":
                    return references.leftUpperArm;
                case "LeftForeArm":
                    return references.leftForearm;
                case "LeftHand":
                    return references.leftHand;
                case "LeftFoot":
                    return references.leftFoot;
                case "LeftToeBase":
                    return references.leftFoot.GetChild(0);
                case "LeftToeEnd":
                    return references.leftFoot.GetChild(0).childCount > 0 ? references.leftFoot.GetChild(0).GetChild(0) : references.leftFoot.GetChild(0);
                case "RightShoulder":
                    return references.rightUpperArm.parent;
                case "RightArm":
                    return references.rightUpperArm;
                case "RightForeArm":
                    return references.rightForearm;
                case "RightHand":
                    return references.rightHand;
                case "RightFoot":
                    return references.rightFoot;
                case "RightToeBase":
                    return references.rightFoot.GetChild(0);
                case "RightToeEnd":
                    return references.rightFoot.GetChild(0).childCount > 0 ? references.rightFoot.GetChild(0).GetChild(0) : references.rightFoot.GetChild(0);
                // ikdollnodes key compatible
                case "LeftElbow":
                    return references.leftForearm;
                case "RightElbow":
                    return references.rightForearm;
                default:
                    return null;
            }
        }

        static public List<Transform> FindChildrenByPartName(Transform person, string name)
        {
            List<Transform> result = new();
            Transform[] armatures = person.GetComponentsInChildren<Transform>();
            foreach (Transform armature in armatures)
            {
                if (armature.name.Contains(name))
                {
                    result.Add(armature);
                }
            }

            return result;
        }

        static public void NormalizeRelative(Transform relative, Transform avatar)
        {
            var leftArm = ArmatureUtils.FindPartString(avatar, "LeftArm");
            var leftForeArm = ArmatureUtils.FindPartString(avatar, "LeftForeArm");
            var leftHand = ArmatureUtils.FindPartString(avatar, "LeftHand");
            var armLength = ((leftForeArm.position - leftArm.position).magnitude +
                (leftHand.position - leftForeArm.position).magnitude);
            Debug.Log("Hello " + avatar.name + ", " + armLength);
            relative.localPosition = relative.localPosition * armLength / _AmyArmLength;
        }

    }
}