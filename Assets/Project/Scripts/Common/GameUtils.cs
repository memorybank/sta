using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playa.Common
{
    public class GameUtils
    {
        private const string toDestroyName = "toDestroy";
        public static string ToDestroyName => toDestroyName;

        public static Transform FindDeepChild(Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName) return c;
                foreach (Transform t in c)
                { 
                    if (t.gameObject.activeSelf && t.name != toDestroyName) 
                    {
                        queue.Enqueue(t);
                    }
                }
            }
            return null;
        }

        public static Component CopyComponent(Component original, GameObject destination)
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;
        }

        public static void RenameDestroy(GameObject toDestroyObj)
        {
            Debug.Log("To Destroy: " + toDestroyObj.name);
            toDestroyObj.name = toDestroyName;
            GameObject.Destroy(toDestroyObj);
        }
    }
}