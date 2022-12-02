using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FbxImporter : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter importer = assetImporter as ModelImporter;
        if (importer != null)
        {
            //importer.animationType = ModelImporterAnimationType.Human;
            Debug.Log("OnPreprocessModel");
        }
    }

    void OnPostprocessGameObjectWithUserProperties(
        GameObject go,
        string[] propNames,
        System.Object[] values)
    {
        Debug.Log("OnPostprocessGameObjectWithUserProperties");

        for (int i = 0; i < propNames.Length; i++)
        {
            string propName = propNames[i];
            System.Object value = (System.Object)values[i];

            Debug.Log("Propname: " + propName + " value: " + values[i]);

            if (value.GetType().ToString() == "System.Int32")
            {
                int myInt = (int)value;
                // do something useful
            }

            // etc...
        }
    }
}
