using Playa.App.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.App
{
    public class StageCmd { }

    public class InstantiateObjectCmd : StageCmd
    {
        public string name;
        public string assetPath;
        public Transform refObj;
        public Vector3 position;
        public Quaternion rotation;
        public SpawnStrategy strategy;
        public List<string> transformNames;

        public InstantiateObjectCmd(string name, string assetPath, Vector3 position, Quaternion rotation, SpawnStrategy strategy, List<string> transformNames)
        {
            this.name = name;
            this.assetPath = assetPath;
            this.position = position;
            this.rotation = rotation;
            this.strategy = strategy;
            this.transformNames = transformNames;
        }
        
        public InstantiateObjectCmd(string name, string assetPath, Vector3 position, Quaternion rotation, SpawnStrategy strategy)
        {
            this.name = name;
            this.assetPath = assetPath;
            this.position = position;
            this.rotation = rotation;
            this.strategy = strategy;
            this.transformNames =  new List<string>();
        }

        public InstantiateObjectCmd(string name, string assetPath, Transform refObj, Vector3 position, Quaternion rotation, SpawnStrategy strategy)
        {
            this.name = name;
            this.assetPath = assetPath;
            this.refObj = refObj;
            this.position = position;
            this.rotation = rotation;
            this.strategy = strategy;
            this.transformNames = new List<string>();
        }
    }
}