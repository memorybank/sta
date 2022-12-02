using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Playa.App;
using UnityEngine.AddressableAssets;
using Playa.Common;
using RootMotion.FinalIK;
using Playa.Avatars;
using Playa.Item;
using finalIK = RootMotion.FinalIK;
using RootMotion;

namespace Playa.App.Stage
{
    using ItemId = UInt64;

    public class StageManager : StageApi
    {
        [SerializeField]private ItemManager _ItemManager;
        // Object

        override public GameObject InstantiateObject(ItemId id, string assetPath, Vector3 position, Quaternion rotation, SpawnStrategy strategy, List<string> transformNames)
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(assetPath).WaitForCompletion();

            // Based on SpawnStrategy, detect collided item
            ItemId collidedItem = 0;
            if (collidedItem != 0)
            {
                if (SpawnStrategy.DoNotOverride == strategy)
                {
                    var item = _ItemManager.FindItemById(collidedItem);
                    if (item != null)
                    {
                        item.Deactivate();
                    }
                }
                return null;
            }

            var inst = Instantiate(prefab, position, rotation);
            PrefabIKHandle(inst, transformNames);

            return inst;
        }

        override public GameObject InstantiateObject(ItemId id, string assetPath, Transform refObj, Vector3 position, Quaternion rotation, SpawnStrategy strategy)
        {
            return InstantiateObject(id, assetPath, refObj.transform.position + position, refObj.transform.rotation * rotation, strategy, new List<string>());
        }

        private void PrefabIKHandle(GameObject itemObj, List<string> transformNames)
        {
            //Transform Amy = itemObj.transform.Find("Armature");
            List<Transform> Amys = ArmatureUtils.FindChildrenByPartName(itemObj.transform, "Armature");
            if (Amys.Count <= 0) return;

            List<GameObject> IKDollNodesList = new();
            for (int i = 0; i < Amys.Count; i++)
            {
                Transform Amy = Amys[i];
                var fullBodyBiped = Amy.gameObject.AddComponent<finalIK.FullBodyBipedIK>();
                BipedReferences.AutoDetectReferences(ref fullBodyBiped.references, Amy, new BipedReferences.AutoDetectParams(true, false));
                GameObject IKDollNodes = new GameObject("IKDollNodes" + i.ToString());                
                IKDollNodes.transform.parent = itemObj.transform;
                IKDollNodes.transform.localPosition = new Vector3();
                IKDollNodes.transform.localRotation = new Quaternion();
                IKDollNodesList.Add(IKDollNodes);

                GameObject posSlot = new GameObject("Slot" + i.ToString());
                posSlot.transform.parent = itemObj.transform;
                posSlot.transform.position = Amys[i].position;
                posSlot.transform.rotation = Amys[i].rotation;
            }

            Transform ikNode;

            if ((ikNode = itemObj.transform.Find("Head")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "Head", "Head");
                }
            }

            if ((ikNode = itemObj.transform.Find("Chest")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "Spine2", "Chest");
                }
            }

            if ((ikNode = itemObj.transform.Find("LeftElbow")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "LeftForeArm", "LeftElbow");
                }
            }

            if ((ikNode = itemObj.transform.Find("LeftHand")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    KeepHand(IKDollNodesList[i], Amys[i], "LeftHand", "LeftHand");
                }
            }

            if ((ikNode = itemObj.transform.Find("RightElbow")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "RightForeArm", "RightElbow");
                }
            }

            if ((ikNode = itemObj.transform.Find("RightHand")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    KeepHand(IKDollNodesList[i], Amys[i], "RightHand", "RightHand");
                }
            }

            if ((ikNode = itemObj.transform.Find("LeftFoot")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "LeftFoot", "LeftFoot");
                }
            }

            if ((ikNode = itemObj.transform.Find("RightFoot")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "RightFoot", "RightFoot");
                }
            }

            if ((ikNode = itemObj.transform.Find("Hip")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "Hips", "Hip");
                }
            }

            if ((ikNode = itemObj.transform.Find("LeftShoulder")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "LeftShoulder", "LeftShoulder");
                }
            }

            if ((ikNode = itemObj.transform.Find("RightShoulder")) != null)
            {
                GameUtils.RenameDestroy(ikNode.gameObject);
                for (int i = 0; i < IKDollNodesList.Count; i++)
                {
                    CreateInstOnIKDollNodes(IKDollNodesList[i], Amys[i], "RightShoulder", "RightShoulder");
                }
            }

            if (transformNames.Count > 0)
            {
                CreateItemRelativeTransform(itemObj, Amys, transformNames);
            }

            foreach (Transform Amy in Amys)
            {
                GameUtils.RenameDestroy(Amy.gameObject);
            }
        }

        private void CreateItemRelativeTransform(GameObject itemObj, List<Transform> Amys, List<string> transformNames)
        {
            for (int i = 0; i < Amys.Count; i++)
            {
                foreach (string transformName in transformNames)
                {
                    GameObject relativeTransform = new GameObject("Relative_" + transformName + i);
                    relativeTransform.transform.parent = itemObj.transform;
                    Transform originalTransform = ArmatureUtils.FindPartString(Amys[i], transformName);
                    Transform originalParent = originalTransform.parent;
                    originalTransform.parent = itemObj.transform;
                    relativeTransform.transform.localPosition = originalTransform.localPosition;
                    relativeTransform.transform.localRotation = originalTransform.localRotation;
                    originalTransform.parent = originalParent;
                }
            }
        }

        private void CreateInstOnIKDollNodes(GameObject IKDollNodes, Transform Amy, string amyBone, string iKName)
        {
            Transform ikNode;

            ikNode = ArmatureUtils.FindPartString(Amy, amyBone);
            GameObject newIKNode = new GameObject("IKDollNodes" + iKName);
            newIKNode.transform.parent = IKDollNodes.transform;
            newIKNode.transform.position = ikNode.position;
            newIKNode.transform.rotation = ikNode.rotation;
        }

        private void KeepHand(GameObject IKDollNodes, Transform Amy, string amyBone,string iKName)
        {
            Transform ikNode;

            ikNode = ArmatureUtils.FindPartString(Amy, amyBone);
            ikNode.parent = IKDollNodes.transform;
            ikNode.name = "IKDollNodes" + iKName;
        }
    }
}