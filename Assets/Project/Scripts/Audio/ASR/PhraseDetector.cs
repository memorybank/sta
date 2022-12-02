using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recognissimo.Core;
using Recognissimo.Components;
using UnityEngine;
using UnityEngine.UI;

using Playa.Common;
using Playa.Audio;
using Playa.NLP;
using TMPro;
using Playa.Audio.Event;

namespace Playa.Audio.ASR
{
    public sealed class PhraseDetector : AudioDetector
    {

        private const string InitializationMessage = "Loading speech model and setup recognizer...";
        private const string GreetingMessage = "Press 'Space' or tap to start/stop recognition";
        private const string RecognitionStartedMessage = "Recognizing...";
        private const string RecognizerCrashedMessage = "Recognizer crashed";

        private SystemLanguage _language;
        private bool _ready;
        private RecognizedText _recognizedText;

        [SerializeField] private Dropdown languageDropdown;
        [SerializeField] private VoskMicrophoneSourceAdapter micSource;

        [SerializeField] private LanguageModelProvider modelProvider;
        [SerializeField] private SpeechRecognizer recognizer;

        [SerializeField] private IdeationalUnitGenerator _IdeationalUnitGenerator;
        public IdeationalUnitEvent ideationalUnitEvent;

        public double _lastRecognizedTextTimestamp;

        // 上一次记录的partial，判断长度用，不需要准
        private string _lastPartialResult = "";

        // UI components
        [SerializeField] private TextMeshProUGUI text;

        public SpeechRecognizer Recognizer => recognizer;

        public SystemLanguage defaultLanguage = SystemLanguage.Unknown;

        private void Awake()
        {
            defaultLanguage = SystemLanguage.Chinese;
            _language = defaultLanguage != SystemLanguage.Unknown
                ? defaultLanguage
                : Application.systemLanguage;

            _recognizedText = new RecognizedText();
        }

        private async void Start()
        {
            text.text = InitializationMessage;

            try
            {
                InitMicSource();
                InitLanguage();
                await InitModelProvider();
                InitLanguageDropdown();
                InitRecognizer();
                OnInitialized();
            }
            catch (Exception e)
            {
                OnError(e.Message);
            }
        }

        public void VoiceActivityUpdateLastRecognizedTextTimestamp(double timestamp)
        {
            _lastRecognizedTextTimestamp = timestamp;
        }

        private void Update()
        {
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

        private void InitMicSource()
        {
            micSource.audioSource = _SpeechSource;
        }

        public override string GetDetectedResult()
        {
            return _recognizedText.CurrentText;
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
            GeneratorIdu(partialResult);
            UpdateUiText();
        }

        private void GeneratorIdu(PartialResult partialResult)
        {
            string[] old_partials = new string[0];
            if (_lastPartialResult != null)
            {
                old_partials = _lastPartialResult.Split(' ');
            }
            string _changingText = partialResult.partial;
            if (_changingText == _lastPartialResult)
            {
                // 不更新情况
                return;
            }
            string[] partials = partialResult.partial.Split(' ');
            _lastPartialResult = _changingText;

            if (old_partials.Length == partials.Length)
            {
                // 不更新情况2
                return;
            }

            float recognizeDuration = 0;
            if (_lastRecognizedTextTimestamp != 0)
            {
                recognizeDuration = (float)(GetMSTimestamp() - _lastRecognizedTextTimestamp);
            }

            string[] partialUnits = partials.Skip(partials.Length - old_partials.Length).ToArray();
            float[] recognizeDurations = new float[partialUnits.Length];
            for (int i = 0; i < partialUnits.Length; i++)
            {
                recognizeDurations[i] = recognizeDuration / (float)partialUnits.Length;
            }

            // length=1 pair generate
            IdeationalUnit idu = _IdeationalUnitGenerator.GenerateUnitWithTextAndDuration(partialUnits, recognizeDurations);
            _lastRecognizedTextTimestamp = GetMSTimestamp();
            ideationalUnitEvent?.Invoke(idu);
        }

        public static double GetMSTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        private class RecognizedText
        {
            private string _changingText;
            private string _stableText;

            public string CurrentText => $"{_changingText}";
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
            }
        }
    }
}