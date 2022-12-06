using Playa.Common;
using RootMotion;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using UnityEngine;
using finalIK = RootMotion.FinalIK;

namespace Playa.Avatars
{
    using ItemId = UInt64;
    public enum IKEffectorName
    {
        LookAt,
        LeftFoot,
        RightFoot,
        LeftHand,
        RightHand,
        LeftHandPoser,
        RightHandPoser,
        LeftElbow,
        RightElbow,
        Hip,
        Chest,
        Root,
        LeftShoulder,
        RightShoulder,
    } 

    public class MultiIKManager : MonoBehaviour
    {
        public bool _Enable;
        public bool _SetIKTriggerPoint = false;
        // Parent pointer
        private AvatarUser _AvatarUser;

        // LookAt Ik is the only IK needed from FinalIk
        [SerializeField] private finalIK.LookAtIK lookatIK;

        private Dictionary<IKEffectorName, GameObject> _TrackingSpheres;

        private Dictionary<IKEffectorName, Transform> _CurrentAvatarIKGoals;

        private IKTarget _BodyIKTarget;

        [SerializeField] private finalIK.CCDIK leftElbowIK;
        [SerializeField] private finalIK.CCDIK rightElbowIK;
        [SerializeField] private finalIK.FullBodyBipedIK fullBodyBipedIK;
        // IK Rig
        [SerializeField] private finalIK.FullBodyBipedIK finalFullBodyBipedIK;
        [SerializeField] private HandPoser lhPoser, rhPoser;
        public bool groundEnable;
        [SerializeField] private GrounderFBBIK groundIK;

        Vector4 targetLeft, targetRight;
       
        private Transform _LookAtTarget;
        private bool _TurnHeadFlag;
        private bool _NeedsReset;
        private float speed = 5.0f;
        private Vector3 _RootOffset = Vector3.zero;

        [SerializeField] private EulerAngleDeformer _Deformer;

        public EulerAngleDeformer Deformer => _Deformer;

        //follow parts
        private Dictionary<ItemId,Dictionary<Transform, Transform>> fakeBodyFollowList = new();

        void Awake()
        {
            _BodyIKTarget = new IKTarget();

            _TrackingSpheres = new Dictionary<IKEffectorName, GameObject> { };
            foreach (IKEffectorName ikEffector in Enum.GetValues(typeof(IKEffectorName)))
            {
                var mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mySphere.name = ikEffector.ToString() + "IKPoint";
                mySphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                mySphere.transform.position = new Vector3(0f, 0f, 0f);
                mySphere.GetComponent<Renderer>().material.color = new Color(0f, 0f, 1f, 0.25f);
                if (ikEffector == IKEffectorName.LookAt)
                {
                    mySphere.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 0.25f);
                }
                mySphere.SetActive(false);
                _TrackingSpheres.Add(ikEffector, mySphere);

            }
           
            _CurrentAvatarIKGoals = new Dictionary<IKEffectorName, Transform>();
            _CurrentAvatarIKGoals[IKEffectorName.Root] = new GameObject(IKEffectorName.Root.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.Hip] = new GameObject(IKEffectorName.Hip.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.LeftShoulder] = new GameObject(IKEffectorName.LeftShoulder.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.RightShoulder] = new GameObject(IKEffectorName.RightShoulder.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.LeftHand] = new GameObject(IKEffectorName.LeftHand.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.LeftElbow] = new GameObject(IKEffectorName.LeftElbow.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.RightHand] = new GameObject(IKEffectorName.RightHand.ToString()).transform;
            _CurrentAvatarIKGoals[IKEffectorName.RightElbow] = new GameObject(IKEffectorName.RightElbow.ToString()).transform;

            CreateIKs();
            
            finalFullBodyBipedIK.solver.bodyEffector.target = _CurrentAvatarIKGoals[IKEffectorName.Hip];
            finalFullBodyBipedIK.solver.leftHandEffector.target = _CurrentAvatarIKGoals[IKEffectorName.LeftHand];
            finalFullBodyBipedIK.solver.leftShoulderEffector.target = _CurrentAvatarIKGoals[IKEffectorName.LeftShoulder];
            //finalFullBodyBipedIK.solver.leftArmChain.bendConstraint.bendGoal = _CurrentAvatarIKGoals[IKEffectorName.LeftElbow];
            finalFullBodyBipedIK.solver.rightHandEffector.target = _CurrentAvatarIKGoals[IKEffectorName.RightHand];
            finalFullBodyBipedIK.solver.rightShoulderEffector.target = _CurrentAvatarIKGoals[IKEffectorName.RightShoulder];
            //finalFullBodyBipedIK.solver.rightArmChain.bendConstraint.bendGoal = _CurrentAvatarIKGoals[IKEffectorName.RightElbow];

            _CurrentAvatarIKGoals[IKEffectorName.LookAt] = lookatIK.transform;

            _Enable = true;
            groundEnable = true;
            lookatIK.enabled = false;
            fullBodyBipedIK.enabled = false;
            leftElbowIK.enabled = false;
            rightElbowIK.enabled = false;
            finalFullBodyBipedIK.enabled = false;
            groundIK.enabled = false;
            targetLeft = new Vector4(1f, 0f, 0f, 0f);
            targetRight = new Vector4(1f, 0f, 0f, 0f);

            _Deformer = new EulerAngleDeformer();
        }

        public void Reset(AvatarUser user)
        {
            _AvatarUser = user;
            _LookAtTarget = lookatIK.solver.target;
            _NeedsReset = true;
            CheckFingersNumber();
        }

        public void SetIKTargets(Dictionary<IKEffectorName, IKTarget> targets)
        {
            foreach (var kvp in targets)
            {
                SetIKTarget(kvp.Key, kvp.Value);
            }
        }

        public void CreateIKs()
        {
            if (gameObject.GetComponent<finalIK.FullBodyBipedIK>() != null)
            {
                return;
            }

            fullBodyBipedIK = gameObject.AddComponent<finalIK.FullBodyBipedIK>();
            BipedReferences.AutoDetectReferences(ref fullBodyBipedIK.references, transform, 
                new BipedReferences.AutoDetectParams(true, false));            
            fullBodyBipedIK.solver.SetToReferences(fullBodyBipedIK.references, fullBodyBipedIK.references.spine[0]);

            finalFullBodyBipedIK = gameObject.AddComponent<finalIK.FullBodyBipedIK>();
            BipedReferences.AutoDetectReferences(ref finalFullBodyBipedIK.references, transform,
                new BipedReferences.AutoDetectParams(true, false));
            finalFullBodyBipedIK.solver.SetToReferences(fullBodyBipedIK.references, finalFullBodyBipedIK.references.spine[0]);

            lookatIK = gameObject.AddComponent<finalIK.LookAtIK>();
            lookatIK.solver.target = new GameObject("IKLookAtProbe").transform;
            lookatIK.solver.target.parent = transform.parent.parent;
            lookatIK.solver.head = new IKSolverLookAt.LookAtBone(finalFullBodyBipedIK.references.head);
            lookatIK.solver.spine = new IKSolverLookAt.LookAtBone[3];
            lookatIK.solver.spine[0] = new IKSolverLookAt.LookAtBone(finalFullBodyBipedIK.references.head);
            lookatIK.solver.spine[1] = new IKSolverLookAt.LookAtBone(finalFullBodyBipedIK.references.head.parent);
            lookatIK.solver.spine[2] = new IKSolverLookAt.LookAtBone(finalFullBodyBipedIK.references.head.parent.parent);

            leftElbowIK = gameObject.AddComponent<finalIK.CCDIK>();
            leftElbowIK.solver.bones = new IKSolverHeuristic.Bone[2];
            leftElbowIK.solver.bones[0] = new IKSolverHeuristic.Bone(finalFullBodyBipedIK.references.leftUpperArm, 1f);
            leftElbowIK.solver.bones[1] = new IKSolverHeuristic.Bone(finalFullBodyBipedIK.references.leftForearm, 1f);
            leftElbowIK.solver.IKPositionWeight = 0f;

            rightElbowIK = gameObject.AddComponent<finalIK.CCDIK>();
            rightElbowIK.solver.bones = new IKSolverHeuristic.Bone[2];
            rightElbowIK.solver.bones[0] = new IKSolverHeuristic.Bone(finalFullBodyBipedIK.references.rightUpperArm, 1f);
            rightElbowIK.solver.bones[1] = new IKSolverHeuristic.Bone(finalFullBodyBipedIK.references.rightForearm, 1f);
            rightElbowIK.solver.IKPositionWeight = 0f;

            groundIK = gameObject.AddComponent<GrounderFBBIK>();
            groundIK.ik = gameObject.AddComponent<FullBodyBipedIK>();
            BipedReferences.AutoDetectReferences(ref groundIK.ik.references, transform,
                new BipedReferences.AutoDetectParams(true, false));
            groundIK.ik.solver.SetToReferences(groundIK.ik.references, groundIK.ik.references.spine[0]);
            groundIK.ik.enabled = false;
            groundIK.solver.layers = 1;
            groundIK.solver.footSpeed = 2.0f;
            groundIK.solver.rotateSolver = true;
            groundIK.solver.pelvisSpeed = 2.0f;


            fullBodyBipedIK.solver.leftArmChain.pull = 0f;
            fullBodyBipedIK.solver.leftArmChain.reach = 0f;
            fullBodyBipedIK.solver.leftArmChain.push = 0f;
            fullBodyBipedIK.solver.rightArmChain.pull = 0f;
            fullBodyBipedIK.solver.rightArmChain.reach = 0f;
            fullBodyBipedIK.solver.rightArmChain.push = 0f;
            fullBodyBipedIK.solver.leftLegChain.pull = 0f;
            fullBodyBipedIK.solver.leftLegChain.reach = 0f;
            fullBodyBipedIK.solver.leftLegChain.push = 0f;
            fullBodyBipedIK.solver.rightLegChain.pull = 0f;
            fullBodyBipedIK.solver.rightLegChain.reach = 0f;
            fullBodyBipedIK.solver.rightLegChain.push = 0f;
            fullBodyBipedIK.solver.headMapping.maintainRotationWeight = 1f;

            finalFullBodyBipedIK.solver.leftArmChain.pull = 0f;
            finalFullBodyBipedIK.solver.leftArmChain.reach = 0f;
            finalFullBodyBipedIK.solver.leftArmChain.push = 0f;
            finalFullBodyBipedIK.solver.rightArmChain.pull = 0f;
            finalFullBodyBipedIK.solver.rightArmChain.reach = 0f;
            finalFullBodyBipedIK.solver.rightArmChain.push = 0f;
            finalFullBodyBipedIK.solver.leftLegChain.pull = 0f;
            finalFullBodyBipedIK.solver.leftLegChain.reach = 0f;
            finalFullBodyBipedIK.solver.leftLegChain.push = 0f;
            finalFullBodyBipedIK.solver.rightLegChain.pull = 0f;
            finalFullBodyBipedIK.solver.rightLegChain.reach = 0f;
            finalFullBodyBipedIK.solver.rightLegChain.push = 0f;
            finalFullBodyBipedIK.solver.headMapping.maintainRotationWeight = 1f;
            finalFullBodyBipedIK.solver.bodyEffector.positionWeight = 1f;
            finalFullBodyBipedIK.solver.leftHandEffector.positionWeight = 1f;
            finalFullBodyBipedIK.solver.leftShoulderEffector.positionWeight = 1f;
            //finalFullBodyBipedIK.solver.leftArmChain.bendConstraint.weight = 1f;
            finalFullBodyBipedIK.solver.rightHandEffector.positionWeight = 1f;
            finalFullBodyBipedIK.solver.rightShoulderEffector.positionWeight = 1f;
            //finalFullBodyBipedIK.solver.rightArmChain.bendConstraint.weight = 1f;
        }


        private void SetIKTarget(IKEffectorName iKEffectorName, IKTarget iktarget)
        {
            if (_SetIKTriggerPoint)
            {
                if (iktarget.target != null)
                {
                    _TrackingSpheres[iKEffectorName].transform.position = iktarget.target.position;
                    _TrackingSpheres[iKEffectorName].SetActive(true);
                }
                else
                {
                    _TrackingSpheres[iKEffectorName].SetActive(false);
                }
            }

            switch (iKEffectorName)
            {
                case IKEffectorName.LeftFoot:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.leftFootEffector.target, iktarget.target));
                        fullBodyBipedIK.solver.leftFootEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.leftFootEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.leftFootEffector.rotationWeight = iktarget.rotationWeight;
                        break;
                    }
                case IKEffectorName.RightFoot:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.rightFootEffector.target, iktarget.target));                        
                        fullBodyBipedIK.solver.rightFootEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.rightFootEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.rightFootEffector.rotationWeight = iktarget.rotationWeight;
                        break;
                    }
                case IKEffectorName.LeftHand:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.leftHandEffector.target, iktarget.target));
                        fullBodyBipedIK.solver.leftHandEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.leftHandEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.leftHandEffector.rotationWeight = iktarget.rotationWeight;
                        Debug.Log("ik Left hand Add");
                        break;
                    }
                case IKEffectorName.RightHand:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.rightHandEffector.target, iktarget.target));
                        fullBodyBipedIK.solver.rightHandEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.rightHandEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.rightHandEffector.rotationWeight = iktarget.rotationWeight;                        
                        Debug.Log("ik Right hand Add");
                        break;
                    }
                case IKEffectorName.LeftHandPoser:
                    {
                        GameObject g = _AvatarUser.ActiveAvatarTransform.gameObject;
                        Transform lh = ArmatureUtils.FindLeftHand(g.transform);
                        if (lh != null)
                        {
                            lhPoser = lh.gameObject.GetComponent<HandPoser>();
                            if (lhPoser == null)
                            {
                                lhPoser = lh.gameObject.AddComponent<HandPoser>();
                            }
                            lhPoser.Disable();

                            Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, lhPoser.poseRoot, iktarget.target));
                            lhPoser.poseRoot = iktarget.target;
                            lhPoser.weight = iktarget.positionWeight;
                            lhPoser.localPositionWeight = 1f;
                        }

                        break;
                    }
                case IKEffectorName.RightHandPoser:
                    {
                        GameObject g = _AvatarUser.ActiveAvatarTransform.gameObject;
                        Transform rh = ArmatureUtils.FindRightHand(g.transform);
                        if (rh != null)
                        {
                            rhPoser = rh.gameObject.GetComponent<HandPoser>();
                            if (rhPoser == null)
                            {
                                rhPoser = rh.gameObject.AddComponent<HandPoser>();
                            }
                            rhPoser.Disable();

                            Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, rhPoser.poseRoot, iktarget.target));
                            rhPoser.poseRoot = iktarget.target;
                            rhPoser.weight = iktarget.positionWeight;
                            rhPoser.localPositionWeight = 1f;
                        }
                        break;
                    }
                case IKEffectorName.LeftElbow:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, leftElbowIK.solver.target, iktarget.target));
                        fullBodyBipedIK.solver.leftArmChain.bendConstraint.bendGoal = iktarget.target;
                        fullBodyBipedIK.solver.leftArmChain.bendConstraint.weight = iktarget.positionWeight;
                        leftElbowIK.solver.target = iktarget.target;
                        leftElbowIK.solver.IKPositionWeight = iktarget.positionWeight;
                        break;
                    }
                case IKEffectorName.RightElbow:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, rightElbowIK.solver.target, iktarget.target));
                        fullBodyBipedIK.solver.rightArmChain.bendConstraint.bendGoal = iktarget.target;
                        fullBodyBipedIK.solver.rightArmChain.bendConstraint.weight = iktarget.positionWeight;
                        rightElbowIK.solver.target = iktarget.target;
                        rightElbowIK.solver.IKPositionWeight = iktarget.positionWeight;
                        break;
                    }
                case IKEffectorName.LeftShoulder:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.leftShoulderEffector.target, iktarget.target));
                        fullBodyBipedIK.solver.leftShoulderEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.leftShoulderEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.leftShoulderEffector.rotationWeight = iktarget.rotationWeight;
                        break;
                    }
                case IKEffectorName.RightShoulder:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.rightShoulderEffector.target, iktarget.target));
                        fullBodyBipedIK.solver.rightShoulderEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.rightShoulderEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.rightShoulderEffector.rotationWeight = iktarget.rotationWeight;
                        break;
                    }
                case IKEffectorName.Hip:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, fullBodyBipedIK.solver.bodyEffector.target, iktarget.target));
                        fullBodyBipedIK.solver.bodyEffector.target = iktarget.target;
                        fullBodyBipedIK.solver.bodyEffector.positionWeight = iktarget.positionWeight;
                        fullBodyBipedIK.solver.bodyEffector.rotationWeight = iktarget.rotationWeight;
                        break;
                    }
                case IKEffectorName.Root:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, _BodyIKTarget, iktarget.target));
                        _BodyIKTarget = iktarget;
                        if (_BodyIKTarget.target == null)
                        {
                            _AvatarUser.ActiveAvatarTransform.localPosition = Vector3.zero;
                            groundEnable = true;
                        }
                        else
                        {
                            groundEnable = false;
                        }
                        break;
                    }
                case IKEffectorName.LookAt:
                    {
                        Debug.Log(string.Format("IKtarget update {0} : {1} => {2}", iKEffectorName, _LookAtTarget, iktarget.target));
                        _LookAtTarget = iktarget.target;

                        lookatIK.solver.IKPositionWeight = 1.0f;
                        lookatIK.solver.bodyWeight = 0.3f;
                        lookatIK.solver.headWeight = 1.0f;
                        lookatIK.solver.eyesWeight = 0.5f;

                        _TurnHeadFlag = iktarget.positionWeight == 0;
                        break;
                    }
                case IKEffectorName.Chest:
                    {
                        Debug.Log("Chest IK control not implemented");
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }   

        public void ClearIKAll()
        {
            foreach (IKEffectorName ikEffectorName in Enum.GetValues(typeof(IKEffectorName)))
            {
                SetIKTarget(ikEffectorName, new IKTarget());
            }
        }

        public void ManualUpdate()
        {
            if (!_Enable) return;

            if (_AvatarUser == null)
            {
                return;
            }
            groundIK.enabled = groundEnable;

            if (_BodyIKTarget.target != null)
            {
                _AvatarUser.ActiveAvatarTransform.position = _AvatarUser.ActiveAvatarTransform.position +
                _BodyIKTarget.target.position - ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "Hips").position;
                // Rotate leg
            }
            else
            {
                groundIK.ik.GetIKSolver().Update();
            }

            HeadIKRigging();

            ComputeWeights();
            _Deformer.Deform(_AvatarUser.ActiveAvatarTransform);
            var animHands = GetHandPositions();
            if (lhPoser != null)
            {
                lhPoser.UpdateManual();
            }
            if (rhPoser != null)
            {
                rhPoser.UpdateManual();
            }
            leftElbowIK.GetIKSolver().Update();
            rightElbowIK.GetIKSolver().Update();
            var ccdHands = GetHandPositions();
            fullBodyBipedIK.GetIKSolver().Update();
            var fbHands = GetHandPositions();
            Interpolate(animHands, ccdHands, fbHands);

            IKRigging();

            //update follow target
            foreach (var pairs in fakeBodyFollowList)
            {
                foreach (var pair in pairs.Value)
                {
                    pair.Key.position = pair.Value.position;
                    pair.Key.rotation = pair.Value.rotation;
                }
            }            
        }

        public void AddFollowers(ItemId _itemId, Transform _follower, Transform _target)
        {
            if (fakeBodyFollowList.ContainsKey(_itemId))
            {
                fakeBodyFollowList[_itemId].Add(_follower, _target);
            }
            else
            {
                Dictionary<Transform, Transform> followPair = new Dictionary<Transform, Transform>();
                followPair.Add(_follower, _target);
                fakeBodyFollowList.Add(_itemId, followPair);
            }
        }

        public void RemoveFollowers(ItemId _itemId)
        {
            if (fakeBodyFollowList.ContainsKey(_itemId))
            {
                fakeBodyFollowList.Remove(_itemId);
            }
        }

        private Dictionary<string, Vector3> GetHandPositions()
        {
            var hands = new Dictionary<string, Vector3>();

            hands.Add("LeftHand", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "LeftHand").position);
            hands.Add("LeftForeArm", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "LeftForeArm").position);
            hands.Add("RightHand", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "RightHand").position);
            hands.Add("RightForeArm", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "RightForeArm").position);
            return hands;
        }

        private Dictionary<string, Vector3> GetHandRotations()
        {
            var hands = new Dictionary<string, Vector3>();

            hands.Add("LeftHand", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "LeftHand").eulerAngles);
            hands.Add("LeftForeArm", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "LeftForeArm").eulerAngles);
            hands.Add("RightHand", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "RightHand").eulerAngles);
            hands.Add("RightForeArm", ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "RightForeArm").eulerAngles);
            return hands;
        }

        private Vector3 GetHeadIKPosition()
        {
            if (_TurnHeadFlag)
            {
                Transform head = ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "Head");
                Vector3 calcuIK = head.position + head.forward;
                return calcuIK;
            }
            else
            {
                return _LookAtTarget.position;
            }
        }

        private void ComputeWeights()
        {
            if (fullBodyBipedIK.solver.leftHandEffector.target != null)
            {
                targetLeft = new Vector3(0f, 0f, 1f);
                Debug.Log(String.Format("{0} using FB left", _AvatarUser.ActiveAnimancerName));
            }
            else if (leftElbowIK.solver.target != null)
            {
                targetLeft = new Vector3(0f, 1f, 0f);
                Debug.Log(String.Format("{0} using CCD left", _AvatarUser.ActiveAnimancerName));
            }
            else
            {
                targetLeft = new Vector3(1f, 0f, 0f);
                Debug.Log(String.Format("{0} using Anim left", _AvatarUser.ActiveAnimancerName));
            }

            if (fullBodyBipedIK.solver.rightHandEffector.target != null)
            {
                targetRight = new Vector3(0f, 0f, 1f);
                Debug.Log(String.Format("{0} using FB right", _AvatarUser.ActiveAnimancerName));
            }
            else if (rightElbowIK.solver.target != null)
            {
                targetRight = new Vector3(0f, 1f, 0f);
                Debug.Log(String.Format("{0} using CCD right", _AvatarUser.ActiveAnimancerName));
            }
            else
            {
                targetRight = new Vector3(1f, 0f, 0f);
                Debug.Log(String.Format("{0} using Anim right", _AvatarUser.ActiveAnimancerName));
            }
        }

        private void Interpolate(Transform target, ref Vector3 current)
        {
            Interpolate(target.position, ref current);
        }
        private void Interpolate(Vector3 target, ref Vector3 current)
        {
            current += (target - current) * Math.Min(Time.deltaTime * speed, 1f) + _RootOffset;
            //current = target;
        }
        private void Interpolate(Dictionary<string, Vector3> animHands, Dictionary<string, Vector3> ccdHands, Dictionary<string, Vector3> fbHands)
        {
            var mat = new Matrix4x4();
            var bone = "";

            bone = "LeftForeArm";
            mat.SetColumn(0, animHands[bone]);
            mat.SetColumn(1, ccdHands[bone]);
            mat.SetColumn(2, fbHands[bone]);
            ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, bone).position = mat * targetLeft;

            bone = "LeftHand";
            mat.SetColumn(0, animHands[bone]);
            mat.SetColumn(1, ccdHands[bone]);
            mat.SetColumn(2, fbHands[bone]);
            ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, bone).position = mat * targetLeft;

            bone = "RightForeArm";
            mat.SetColumn(0, animHands[bone]);
            mat.SetColumn(1, ccdHands[bone]);
            mat.SetColumn(2, fbHands[bone]);
            ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, bone).position = mat * targetRight;

            bone = "RightHand";
            mat.SetColumn(0, animHands[bone]);
            mat.SetColumn(1, ccdHands[bone]);
            mat.SetColumn(2, fbHands[bone]);
            ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, bone).position = mat * targetRight;
        }

        private void HeadIKRigging()
        {
            Vector3 current = lookatIK.solver.target.position;
            Interpolate(GetHeadIKPosition(), ref current);
            lookatIK.solver.target.position = current;
            lookatIK.solver.Update();
        }

        private void IKRigging()
        {
            var finalHands = GetHandPositions();
            // Plan transition between IK and anims
            if (_NeedsReset)
            {
                _NeedsReset = false; 
                _CurrentAvatarIKGoals[IKEffectorName.LeftHand].position = finalHands["LeftHand"];
                _CurrentAvatarIKGoals[IKEffectorName.LeftElbow].position = finalHands["LeftForeArm"];
                _CurrentAvatarIKGoals[IKEffectorName.RightHand].position = finalHands["RightHand"];
                _CurrentAvatarIKGoals[IKEffectorName.RightElbow].position = finalHands["RightForeArm"];
                _CurrentAvatarIKGoals[IKEffectorName.Root].position = _AvatarUser.ActiveAvatarTransform.position;
                return;
            }


            // Filter out avatar root motion portion
            var delta = _AvatarUser.ActiveAvatarTransform.position - _CurrentAvatarIKGoals[IKEffectorName.Root].position;

            var finalVec = new Vector3();
            _CurrentAvatarIKGoals[IKEffectorName.LeftHand].position += delta;
            finalVec = _CurrentAvatarIKGoals[IKEffectorName.LeftHand].position;
            Interpolate(finalHands["LeftHand"], ref finalVec);
            _CurrentAvatarIKGoals[IKEffectorName.LeftHand].position = finalVec;
            _CurrentAvatarIKGoals[IKEffectorName.LeftElbow].position += delta;
            finalVec = _CurrentAvatarIKGoals[IKEffectorName.LeftElbow].position;
            Interpolate(finalHands["LeftForeArm"], ref finalVec);
            _CurrentAvatarIKGoals[IKEffectorName.LeftElbow].position = finalVec;
            _CurrentAvatarIKGoals[IKEffectorName.RightHand].position += delta;
            finalVec = _CurrentAvatarIKGoals[IKEffectorName.RightHand].position;
            Interpolate(finalHands["RightHand"], ref finalVec);
            _CurrentAvatarIKGoals[IKEffectorName.RightHand].position = finalVec;
            _CurrentAvatarIKGoals[IKEffectorName.RightElbow].position += delta;
            finalVec = _CurrentAvatarIKGoals[IKEffectorName.RightElbow].position;
            Interpolate(finalHands["RightForeArm"], ref finalVec);
            _CurrentAvatarIKGoals[IKEffectorName.RightElbow].position = finalVec;

            _CurrentAvatarIKGoals[IKEffectorName.Root].position = _AvatarUser.ActiveAvatarTransform.position;
            _CurrentAvatarIKGoals[IKEffectorName.Hip].position = ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "Spine").position;
            _CurrentAvatarIKGoals[IKEffectorName.LeftShoulder].position = ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "LeftShoulder").position;
            _CurrentAvatarIKGoals[IKEffectorName.RightShoulder].position = ArmatureUtils.FindPartString(_AvatarUser.ActiveAvatarTransform, "RightShoulder").position;

            finalHands = GetHandRotations();
            finalVec = _CurrentAvatarIKGoals[IKEffectorName.LeftHand].eulerAngles;
            Interpolate(finalHands["LeftHand"], ref finalVec);
            _CurrentAvatarIKGoals[IKEffectorName.LeftHand].eulerAngles = finalVec;
            finalVec = _CurrentAvatarIKGoals[IKEffectorName.RightHand].eulerAngles;
            Interpolate(finalHands["RightHand"], ref finalVec);
            _CurrentAvatarIKGoals[IKEffectorName.RightHand].eulerAngles = finalVec;

            finalFullBodyBipedIK.GetIKSolver().Update();
        }

        private void CheckFingersNumber()
        {
            Animator animator = _AvatarUser.ActiveAvatarTransform.transform.GetComponent<Animator>();
            Transform finger;

            Transform leftThumb = animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
            Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            CompleteFinger(animator.GetBoneTransform(HumanBodyBones.LeftThumbProximal));
            CompleteFinger(animator.GetBoneTransform(HumanBodyBones.LeftIndexProximal));
            finger = animator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
            if (finger == null)
            {
                leftThumb.parent = _AvatarUser.ActiveAvatarTransform;
                GameObject middleFinger = new GameObject("FillerMiddle");
                middleFinger.transform.parent = leftHand;
                leftThumb.parent = leftHand;
                finger = middleFinger.transform;
            }
            CompleteFinger(finger);
            finger = animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
            if (finger == null)
            {
                leftThumb.parent = _AvatarUser.ActiveAvatarTransform;
                GameObject pinkyFinger = new GameObject("FillerPinky");
                GameObject ringFinger = new GameObject("FillerRing");
                pinkyFinger.transform.parent = leftHand;
                ringFinger.transform.parent = leftHand;
                leftThumb.parent = leftHand;
                CompleteFinger(pinkyFinger.transform);
                CompleteFinger(ringFinger.transform);
            }
            else
            {
                CompleteFinger(animator.GetBoneTransform(HumanBodyBones.LeftLittleProximal));
                CompleteFinger(animator.GetBoneTransform(HumanBodyBones.LeftRingProximal));
            }

            Transform rightThumb = animator.GetBoneTransform(HumanBodyBones.RightThumbProximal);
            Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            CompleteFinger(animator.GetBoneTransform(HumanBodyBones.RightThumbProximal));
            CompleteFinger(animator.GetBoneTransform(HumanBodyBones.RightIndexProximal));
            finger = animator.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
            if (finger == null)
            {
                rightThumb.parent = _AvatarUser.ActiveAvatarTransform;
                GameObject middleFinger = new GameObject("FillerMiddle");
                middleFinger.transform.parent = rightHand;
                rightThumb.parent = rightHand;
                finger = middleFinger.transform;
            }
            CompleteFinger(finger);
            finger = animator.GetBoneTransform(HumanBodyBones.RightLittleProximal);
            if (finger == null)
            {
                rightThumb.parent = _AvatarUser.ActiveAvatarTransform;
                GameObject pinkyFinger = new GameObject("FillerPinky");
                GameObject ringFinger = new GameObject("FillerRing");
                pinkyFinger.transform.parent = rightHand;
                ringFinger.transform.parent = rightHand;
                rightThumb.parent = rightHand;
                CompleteFinger(pinkyFinger.transform);
                CompleteFinger(ringFinger.transform);
            }
            else
            {
                CompleteFinger(animator.GetBoneTransform(HumanBodyBones.RightLittleProximal));
                CompleteFinger(animator.GetBoneTransform(HumanBodyBones.RightRingProximal));
            }

        }

        private void CompleteFinger(Transform finger)
        {
            for (int i = 0; i < 4; i++)
            {
                if (finger.childCount == 0)
                {
                    GameObject filler = new GameObject("filler" + i);
                    filler.transform.parent = finger;
                }
                finger = finger.GetChild(0);
            }
            
            if (finger.childCount > 0)
            {
                GameUtils.RenameDestroy(finger.GetChild(0).gameObject);
            }
        }
    }
}
