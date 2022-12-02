using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Playa.App;

namespace Playa.App.Stage
{
    using ItemId = UInt64;

    public enum SpawnStrategy
    {
        Default = 0,
        Override = 1,
        DoNotOverride = 2,
    }

    public abstract class StageApi : MonoBehaviour
    {
        abstract public GameObject InstantiateObject(ItemId id, string assetPath, Vector3 position, Quaternion rotation, SpawnStrategy strategy, List<string> transformNames);
        abstract public GameObject InstantiateObject(ItemId id, string assetPath, Transform refObj, Vector3 position, Quaternion rotation, SpawnStrategy strategy);
    }
}