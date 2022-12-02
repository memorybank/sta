using Playa.Common;
using UnityEngine;

namespace Playa.NLP
{
    public abstract class IdeationalUnitGenerator : MonoBehaviour
    {
        [SerializeField] private string _ModelPath;

        protected string _ModelFilePath;

        void Awake()
        {
#if UNITY_EDITOR
            _ModelFilePath = Application.dataPath + "/StreamingAssets/" + _ModelPath;
            Debug.Log("Unity Editor");
#elif UNITY_STANDALONE_WIN
            _ModelFilePath = Application.dataPath + "/StreamingAssets/" + _ModelPath;
            Debug.Log("Unity Win");
#elif UNITY_STANDALONE_OSX
            _ModelFilePath = Application.dataPath + "/Resources/Data/StreamingAssets/" + _ModelPath;
            Debug.Log("Unity OSX");
#else
#endif
            Init();
        }

        abstract public void Init();

        abstract public IdeationalUnit GenerateUnitWithTextAndDuration(string[] sent, float[] duration);

        abstract public IdeationalUnit GenerateUnitWithTextAndDuration(string sent2, float duration2);
    }
}
