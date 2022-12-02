using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recognissimo.Core;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.UI;

using Playa.Common;
using Playa.NLP;

namespace Playa.Audio.ASR
{
    public sealed class IdeationalUnitRecognizer : MonoBehaviour
    {

        private const string InitializationMessage = "Loading speech model and setup recognizer...";
        private const string GreetingMessage = "Press 'Space' or tap to start/stop recognition";
        private const string RecognitionStartedMessage = "Recognizing...";
        private const string RecognizerCrashedMessage = "Recognizer crashed";

        private SystemLanguage _language;
        private bool _ready;
        private RecognizedText _recognizedText;
        public int _MaxRecognizedTextPartialClipCount;

        [SerializeField] private Dropdown languageDropdown;
        [SerializeField] private MicrophoneSpeechSource micSource;
        [SerializeField] private LanguageModelProvider modelProvider;
        [SerializeField] private SpeechRecognizer recognizer;
        [SerializeField] private IdeationalUnitGenerator _IdeationalUnitGenerator;
        [SerializeField] private Dropdown partialParamDropdown;
        // UI components
        [SerializeField] private Text text;
        [SerializeField] private Text grammarTree;
        [SerializeField] private Text gestureSequence;

        public IdeationalUnitEvent ideationalUnitEvent;

        public int _MaxPartialResultCount;
        public List<string> _GeneratedPartialTextList;
        public List<float> _GeneratedPartialDurationList;
        public double _lastRecognizedTextTimestamp;
        public bool _isTextContinuouslyRecognizing;

        private string _lastPartialResult;

        public SpeechRecognizer Recognizer => recognizer;

        public SystemLanguage defaultLanguage = SystemLanguage.Unknown;

        [Serializable]
        public class IdeationalUnitEvent : UnityEvent<IdeationalUnit>
        {
        }

        private void Awake()
        {
            defaultLanguage = SystemLanguage.Chinese;
            _language = defaultLanguage != SystemLanguage.Unknown
                ? defaultLanguage
                : Application.systemLanguage;

            _recognizedText = new RecognizedText();
            _GeneratedPartialTextList = new List<string>();
            _GeneratedPartialDurationList = new List<float>();
        }

        private async void Start()
        {
            text.text = InitializationMessage;

            try
            {
                await InitPlatformPermissions();
                InitMicSource();
                InitLanguage();
                await InitModelProvider();
                InitLanguageDropdown();
                InitRecognizer();
                InitPartialParamDropdown();
                OnInitialized();
            }
            catch (Exception e)
            {
                OnError(e.Message);
            }
        }

        public void UpdateIdu(IdeationalUnit idu)
        {
            if (idu != null && idu.Phrases.Count > 0)
            {
                UpdateUiText();
                ideationalUnitEvent?.Invoke(idu);
            }
        }

        private void Update()
        {
            IdeationalUnit idu = UpdateIdeationalUnitGenerateDuration();
            if (idu != null && idu.Phrases.Count > 0)
            {
                UpdateUiText();
                ideationalUnitEvent?.Invoke(idu);
            }

            if (!_ready)
            {
                return;
            }

            var spaceReleased = Input.GetKeyUp(KeyCode.Space);
            var touchEnded = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;

            if (!spaceReleased && !touchEnded)
            {
                return;
            }

            if (recognizer.IsRecognizing)
            {
                OnStop();
            }
            else
            {
                OnStart();
            }
        }

        private static async Task InitPlatformPermissions()
        {
#if UNITY_IOS
            await PlatformPermissions.RequestIOSPermissions();
#elif UNITY_ANDROID
            PlatformPermissions.RequestAndroidPermissions();
#endif
            await Task.CompletedTask;
        }

        private void InitMicSource()
        {
            micSource.microphoneSettings.deviceIndex = 0;
            micSource.microphoneSettings.sampleRate = 16000;
            micSource.StartMicrophone();
        }

        private void InitLanguage()
        {
            if (modelProvider.speechModels.Any(info => info.language == _language))
            {
                return;
            }

            const SystemLanguage fallbackLanguage = SystemLanguage.English;

            Debug.LogWarning($"Fallback from {_language.ToString()} to {fallbackLanguage}");

            _language = fallbackLanguage;
        }

        private async Task InitModelProvider()
        {
            await modelProvider.InitializeAsync();
            await modelProvider.LoadLanguageModelAsync(_language);
        }

        private void InitLanguageDropdown()
        {
            languageDropdown.options = modelProvider.speechModels
                .Select(info => new Dropdown.OptionData { text = info.language.ToString() })
                .ToList();

            languageDropdown.value =
                languageDropdown.options.FindIndex(option => option.text == _language.ToString());

            languageDropdown.onValueChanged.AddListener(index =>
            {
                var optionText = languageDropdown.options[index].text;
                var selectedLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), optionText);
                OnLanguageChanged(selectedLanguage);
            });
        }

        private void InitPartialParamDropdown()
        {
            List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();
            for(int i=1;i<=5;i++)
            {
                optionDatas.Add(new Dropdown.OptionData(i.ToString()));
            }

            partialParamDropdown.options = optionDatas;

            partialParamDropdown.value =
                partialParamDropdown.options.FindIndex(option => option.text == "");

            partialParamDropdown.onValueChanged.AddListener(index =>
            {
                var optionText = partialParamDropdown.options[index].text;
                OnPartialResultParamChanged((int)Convert.ToUInt32(optionText));
            });
        }

        private void InitRecognizer()
        {
            recognizer.speechSource = micSource;
            recognizer.modelProvider = modelProvider;
            recognizer.partialResultReady.AddListener(OnPartialResultReady);
            recognizer.resultReady.AddListener(OnResultReady);
            recognizer.crashed.AddListener(OnCrashed);
        }

        private void UpdateUiText()
        {
            text.text = _recognizedText.CurrentText;
            grammarTree.text = _recognizedText.GrammarText;
            gestureSequence.text = _recognizedText.GestureText;
        }

        private void OnPartialResultParamChanged(int index)
        {
            Debug.Assert(index > 0);
            _MaxPartialResultCount = index;
        }

        private void OnStart()
        {
            _recognizedText.Clear();

            text.text = RecognitionStartedMessage;
            recognizer.modelProvider = modelProvider;

            try
            {
                recognizer.StartRecognition();
            }
            catch (Exception e)
            {
                text.text = e.Message;
            }
        }

        private void OnStop()
        {
            text.text = GreetingMessage;
            recognizer.StopRecognition();
        }

        private void OnInitialized()
        {
            text.text = GreetingMessage;
            _ready = true;
        }

        private void OnError(string error)
        {
            if (text != null)
            {
                text.text = error;
            }
        }

        private void OnCrashed()
        {
            OnError(RecognizerCrashedMessage);
        }

        private async void OnLanguageChanged(SystemLanguage language)
        {
            _ready = false;

            recognizer.StopRecognition();
            text.text = InitializationMessage;
            await modelProvider.LoadLanguageModelAsync(language);
            OnInitialized();
        }

        private void OnResultReady(Result result)
        {
            _recognizedText.Add(result);
            UpdateUiText();
        }

        private void OnPartialResultReady(PartialResult partialResult)
        {
            _recognizedText.Add(partialResult);
            UpdateGeneratorBuffer(partialResult);
            UpdateUiText();
        }

        private void UpdateGeneratorBuffer(PartialResult partialResult)
        {
            string _changingText = partialResult.partial;
            if (_changingText == _lastPartialResult)
            {
                // 不更新情况
                return;
            }
            _lastPartialResult = _changingText;

            string[] partials = partialResult.partial.Split(' ');
            string[] old_partials = _lastPartialResult.Split(' ');
            if (partials.Length > 1 && partials.Length > old_partials.Length + 1)
            {
                Debug.LogWarning("IdeationalUnitRecognizer.UpdateGeneratorBuffer partial result > 1");
                // do nothing
            }

            if (_GeneratedPartialTextList.Count > 0 &&
                partials.Length >= _GeneratedPartialTextList.Count &&
                Enumerable.SequenceEqual(partials.Skip(partials.Length - _GeneratedPartialTextList.Count), _GeneratedPartialTextList))
            {
                // 不更新情况2
                return;
            }

            _GeneratedPartialTextList.Add(partials[partials.Length - 1]);
            
            _GeneratedPartialDurationList.Add((float)(GetMSTimestamp() - _lastRecognizedTextTimestamp));

            _lastRecognizedTextTimestamp = GetMSTimestamp();
        }
        
        public IdeationalUnit UpdateIdeationalUnitGenerateDuration()
        {
            // 更新时间戳
            double oldLastRecognizedTextTimestamp = _lastRecognizedTextTimestamp;
            if (!_isTextContinuouslyRecognizing)
            {
                _lastRecognizedTextTimestamp = GetMSTimestamp();
            }

            // 判断
            if (_GeneratedPartialTextList.Count > 0)
            {
                bool isNotSpeakRecognized = false;
                if (oldLastRecognizedTextTimestamp + 1000 < GetMSTimestamp())
                {
                    isNotSpeakRecognized = true;
                }
                if (isNotSpeakRecognized || _GeneratedPartialTextList.Count >= _MaxPartialResultCount)
                {
                    IdeationalUnit idu = _IdeationalUnitGenerator.GenerateUnitWithTextAndDuration(_GeneratedPartialTextList.ToArray(), _GeneratedPartialDurationList.ToArray());
                    string gst = "";
                    foreach (Phrase f in idu.Phrases)
                    {
                        gst += f.Text + " " + f.Duration.ToString() + "ms  ";
                    }
                    _recognizedText._gestureText = gst;


                    _GeneratedPartialTextList.Clear();
                    _GeneratedPartialTextList.TrimExcess();
                    _GeneratedPartialDurationList.Clear();
                    _GeneratedPartialDurationList.TrimExcess();
                    if (isNotSpeakRecognized)
                    {
                        _isTextContinuouslyRecognizing = false;
                    }

                    return idu;
                }
            }

            return null;
        }

        public static double GetMSTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        private class RecognizedText
        {
            private string _changingText;
            private string _stableText;
            private string _grammarText;
            public string _gestureText;

            public string CurrentText => $"{_stableText} <color=red>{_changingText}</color>";
            public string GrammarText => _grammarText;
            public string GestureText => _gestureText;

            public void Add(Result result)
            {
                _changingText = "";
                // _stableText = $"{_stableText} {result.text}";
                _stableText = $"{result.text}";
            }

            public void Add(PartialResult partialResult)
            {
                _changingText = partialResult.partial;
            }

            public void Clear()
            {
                _changingText = "";
                _stableText = "";
                _grammarText = "";
                _gestureText = "";
            }
        }

        private static class PlatformPermissions
        {
            public static async Task RequestIOSPermissions()
            {
                var result = Application.RequestUserAuthorization(UserAuthorization.Microphone);
                var isComplete = new TaskCompletionSource<bool>();
                result.completed += operation =>
                {
                    if (operation.isDone)
                    {
                        isComplete.SetResult(operation.isDone);
                    }
                    else
                    {
                        isComplete.SetException(new InvalidOperationException("Microphone access denied"));
                    }
                };

                await isComplete.Task;
            }

            public static void RequestAndroidPermissions()
            {
                var requestedPermissions = new List<string>
                    {Permission.Microphone, Permission.ExternalStorageWrite, Permission.ExternalStorageRead};

                List<string> FindMissingPermissions() =>
                    requestedPermissions.FindAll(permission => !Permission.HasUserAuthorizedPermission(permission));

                FindMissingPermissions().ForEach(Permission.RequestUserPermission);

                if (FindMissingPermissions().Count > 0)
                {
                    throw new InvalidOperationException("Permission request failed");
                }
            }
        }
    }

}