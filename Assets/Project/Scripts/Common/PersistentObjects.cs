using UnityEngine;
using System.Collections.Generic;

namespace Playa.Common
{
    public class PersistentObjects : MonoBehaviour
    {
        public static PersistentObjects Instance;
        void Awake()
        {
            // Naive way to implement singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;                         // linking the self-reference
            DontDestroyOnLoad(transform.gameObject); // set to dont destroy
        }
    }

}