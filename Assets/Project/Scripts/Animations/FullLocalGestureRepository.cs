using System.Collections;
using System.Collections.Generic;
using Playa.Common;
using UnityEngine;
using UnityEditor;
using System.IO;
using Playa.Common.Utils;
using Playa.Config;
using Animancer;

namespace Playa.Animations
{

    public class FullLocalGestureRepository : AnimationRepository
    {
        [SerializeField] private List<string> _AssetPath;

        [SerializeField] private List<cfg.gesture.Phase> _Phases;

        [SerializeField] private ConfigsLoader _ConfigLoader;

        public List<string> AssetPath => _AssetPath;
        public override void Init()
        {
            _ConfigLoader = ConfigsLoader.Instance;

            foreach (var assetPath in _AssetPath)
            {
                var path = Application.dataPath + "/" + assetPath;

#if UNITY_EDITOR
                List<string> dirPaths = FileUtils.GetAllSubDirsWithSuffix(path, ".fbx");
                foreach (string dirPath in dirPaths)
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(dirPath);
                    foreach (Object obj in assets)
                    {
                        if (obj is AnimationClip)
                        {
                            if (obj.name.Contains("__preview__"))
                            {
                                continue;
                            }

                            ClipTransition clip = new ClipTransition();
                            EditorUtility.CopySerialized(obj, clip.Clip);
                            string name = Path.GetFileNameWithoutExtension(dirPath);

                            AddAnimationClip(clip);
                            clip.Clip.name = name;
                        }
                    }
                }      

#else
#endif
                for (int i = 0; i < _ConfigLoader.Tables.TbGestureMark.DataList.Count; i++)
                {
                    if (_Phases.Contains(_ConfigLoader.Tables.TbGestureMark.DataList[i].Phase))
                    {
                        var clipInfo = new GestureClipInfo();
                        clipInfo.GestureMark = _ConfigLoader.Tables.TbGestureMark.DataList[i];
                        clipInfo.Id = _ConfigLoader.Tables.TbGestureMark.DataList[i].Id;
                        clipInfo.ClipName = clipInfo.GestureMark.File.Split('.')[0];
                        AddAnimationClipInfo(clipInfo);
                    }
                }
                GenerateIndices();
            }
        }
    }
}