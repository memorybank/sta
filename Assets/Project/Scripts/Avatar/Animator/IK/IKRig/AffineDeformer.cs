using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Playa.Common;
using Animancer;

namespace Playa.Avatars.IK
{
    public class AffineDeformer : IKRigDeformer
    {
        private const float _Lambda = 1f;

        Matrix4x4 _AffineTrans;

        public AffineDeformer(Transform reference, List<KeyFrame> keyFrames) : base(reference)
        {
            Debug.Assert(keyFrames.Count >= 2, "not enough support key frames");

            var animancerComponent = reference.GetComponent<AnimancerComponent>();
            var state = animancerComponent.Layers[0].CurrentState;

            var origin = new List<Vector4>();
            for (int i = 0; i < keyFrames.Count; i++)
            {                
                state.Time = keyFrames[i].originTime;
                animancerComponent.Evaluate();
                var pose = NormalizedIKPose.GetCurrentPose(_Avatar);
                origin.Add(ToAffine(pose.LeftHandPosition));
                origin.Add(ToAffine(keyFrames[i].originVelocity));
            }
            
            state.Time = 0f;

            var dest = new List<Vector4>();
            for (int i = 0; i < keyFrames.Count; i++)
            {
                dest.Add(ToAffine(keyFrames[i].destPosition));
                dest.Add(ToAffine(keyFrames[i].destVelocity));
            }

            var destMulOrigin = new Matrix4x4();
            for (int i = 0; i < 2 * keyFrames.Count; i++)
            {
                Acc(ref destMulOrigin, OuterProduct(dest[i], origin[i]));
            }

            var originMulOrigin = new Matrix4x4();
            for (int i = 0; i < 2 * keyFrames.Count; i++)
            {
                Acc(ref originMulOrigin, OuterProduct(origin[i], origin[i]));                
            }

            originMulOrigin[0, 0] += _Lambda * (keyFrames.Count - 2);
            originMulOrigin[1, 1] += _Lambda * (keyFrames.Count - 2);
            originMulOrigin[2, 2] += _Lambda * (keyFrames.Count - 2);
            originMulOrigin[3, 3] += _Lambda * (keyFrames.Count - 2);
                        
            _AffineTrans = destMulOrigin * originMulOrigin.inverse.transpose;

            Debug.Log("destMulOrigin\n" + destMulOrigin);
            Debug.Log("originMulOrigin\n" + originMulOrigin);
            Debug.Log("Affine\n" + _AffineTrans);
        }

        public override NormalizedIKPose Deform(NormalizedIKPose pose)
        {
            var deformed = pose.Copy();
            
            deformed.LeftHandPosition = _AffineTrans * ToAffine(deformed.LeftHandPosition);

            return deformed;
        }

        private Vector4 ToAffine(Vector3 vec)
        {
            Vector4 result = vec;
            result.w = 1f;
            return result;
        }

        private Matrix4x4 OuterProduct(Vector4 a, Vector4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            result[0, 0] = a.x * b.x;
            result[0, 1] = a.x * b.y;
            result[0, 2] = a.x * b.z;
            result[0, 3] = a.x * b.w;
            result[1, 0] = a.y * b.x;
            result[1, 1] = a.y * b.y;
            result[1, 2] = a.y * b.z;
            result[1, 3] = a.y * b.w;
            result[2, 0] = a.z * b.x;
            result[2, 1] = a.z * b.y;
            result[2, 2] = a.z * b.z;
            result[2, 3] = a.z * b.w;
            result[3, 0] = a.w * b.x;
            result[3, 1] = a.w * b.y;
            result[3, 2] = a.w * b.z;
            result[3, 3] = a.w * b.w;
            return result;
        }

        private void Acc(ref Matrix4x4 a, Matrix4x4 b)
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    a[i, j] += b[i, j];
                }
            }
        }
    }
}