using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Avatars
{
    abstract public class PoseDeformer
    {
        abstract public void Deform(Transform armature);
    }
}