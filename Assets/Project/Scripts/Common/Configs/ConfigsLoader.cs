using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

namespace Playa.Config
{

    public class ConfigsLoader : MonoBehaviour
    {
        [SerializeField] private string _GameConfDir;

        [SerializeField] private cfg.Tables _Tables;

        // Singleton
        private static ConfigsLoader _Instance = null;

        public static ConfigsLoader Instance
        { 
            get
            {
                // This may not be thread-safe
                if (_Instance == null)
                {
                    _Instance = new GameObject("Config").AddComponent<ConfigsLoader>();
                    _Instance._GameConfDir = Application.streamingAssetsPath + "/Configs/tables/GenerateDatas/json";

                    _Instance._Tables = new cfg.Tables(
                        file => JSON.Parse(File.ReadAllText(string.Format("{0}/{1}.json", _Instance._GameConfDir, file))));
                    Debug.Log(string.Format("Config loaded {0} gestures", _Instance._Tables.TbGestureMark.DataList.Count));
                    Debug.Log(string.Format("Config loaded {0} gesture sequences", _Instance._Tables.TbGestureSequence.DataList.Count));
                }
                return _Instance;
            }
        }

        public cfg.Tables Tables => _Tables;

        private void Awake()
        {
        }
    }
}