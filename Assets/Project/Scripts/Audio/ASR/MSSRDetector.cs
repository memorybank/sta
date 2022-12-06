//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System.Threading.Tasks;
using System.Globalization;
using System;
using System.Diagnostics;
using Playa.NLP.Parser;
using TMPro;
using Playa.Common;
using Playa.Common.Utils;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
using Debug = UnityEngine.Debug;

namespace Playa.Audio.ASR
{
    class MSSRDetector : AudioDetector
    {
        private bool isRecording;
		[SerializeField] private NaturalLanguageParser _NaturalLanguageParser;

		public NaturalLanguageParser NaturalLanguageParser => _NaturalLanguageParser;

		public string fromLanguage;

		public bool IsTranslationEnabled;

		public List<string> TranslateToLanguages = new();

		public float _lastResultEndTime = 0.0f;

		private Playa.Common.Utils.Timer _Timer;

		// UI components
		[SerializeField] private TextMeshProUGUI _resultText;
		[SerializeField] private TextMeshProUGUI _latencyTracker;

        // the recognition events are raised in a separate thread
        private string recognizedString = "";
        private System.Object threadLocker = new System.Object();

		// Speech recognition key, required
		[Tooltip("Connection string to Cognitive Services Speech.")]
		public string SpeechServiceAPIKey = string.Empty;
		[Tooltip("Region for your Cognitive Services Speech instance (must match the key).")]
		public string SpeechServiceRegion = "westus";

		// Cognitive Services Speech objects used for Speech Recognition
		private SpeechRecognizer recognizer;
		private TranslationRecognizer translator;

		private bool micPermissionGranted = false;
#if PLATFORM_ANDROID
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

        private void Awake()
        {
            // IMPORTANT INFO BEFORE YOU CAN USE THIS SAMPLE:
            // Get your own Cognitive Services Speech subscription key for free at the following
            // link: https://docs.microsoft.com/azure/cognitive-services/speech-service/get-started.
            // Use the inspector fields to manually set these values with your subscription info.
            // If you prefer to manually set your Speech Service API Key and Region in code,
            // then uncomment the two lines below and set the values to your own.
            //SpeechServiceAPIKey = "YourSubscriptionKey";
            //SpeechServiceRegion = "YourServiceRegion";
        }

        private void Start()
        {
#if PLATFORM_ANDROID
        // Request to use the microphone, cf.
        // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
        recognizedString = "Waiting for microphone permission...";
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#else
            micPermissionGranted = true;
#endif
            recognizedString = "Ready";

            _NaturalLanguageParser.Init();
        }

        /// <summary>
        /// Attach to button component used to launch continuous recognition (with or without translation)
        /// </summary>
        private void StartContinuous()
        {
            recognizedString = "";
            if (_SpeechSource.audioSourceType == Agora.AudioSourceType.Remote)
            {
                return;
            }
            if (micPermissionGranted)
            {
                if (IsTranslationEnabled)
                {
                    StartContinuousTranslation();
                }
                else
                {
                    StartContinuousRecognition();
                }
            }
            else
            {
                Debug.LogError("This app cannot function without access to the microphone." + recognizedString);
            }
        }

        void Update()
        {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
        }
#endif
            // Used to update results on screen during updates
            lock (threadLocker)
            {
                _resultText.text = recognizedString;
            }

            // space check .no space will return
            var spaceReleased = Input.GetKeyUp(KeyCode.Space);
            var touchEnded = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;

            if (!spaceReleased && !touchEnded)
            {
                return;
            }

            if (isRecording)
            {
                StopRecognition();
            }
            else
            {
                StartContinuous();
                //TimeUtils.StampCurrentTime("[MSSpeechDetector Test]Start Microphone ");
            }
        }

        /// <summary>
        /// Creates a class-level Speech Recognizer for a specific language using Azure credentials
        /// and hooks-up lifecycle & recognition events
        /// </summary>
        void CreateSpeechRecognizer()
        {
            if (SpeechServiceAPIKey.Length == 0 || SpeechServiceAPIKey == "YourSubscriptionKey")
            {
                Debug.LogError("You forgot to obtain Cognitive Services Speech credentials and inserting them in this app." + Environment.NewLine +
                                   "See the README file and/or the instructions in the Awake() function for more info before proceeding.");
                return;
            }
            Debug.Log("Creating Speech Recognizer.");

            if (recognizer == null)
            {
                SpeechConfig config = SpeechConfig.FromSubscription(SpeechServiceAPIKey, SpeechServiceRegion);
                config.SpeechRecognitionLanguage = fromLanguage;
                recognizer = new SpeechRecognizer(config);

                if (recognizer != null)
                {
                    // Subscribes to speech events.
                    recognizer.Recognizing += RecognizingHandler;
                    recognizer.Recognized += RecognizedHandler;
                    recognizer.SpeechStartDetected += SpeechStartDetectedHandler;
                    recognizer.SpeechEndDetected += SpeechEndDetectedHandler;
                    recognizer.Canceled += CanceledHandler;
                    recognizer.SessionStarted += SessionStartedHandler;
                    recognizer.SessionStopped += SessionStoppedHandler;
                }
            }
            Debug.Log("CreateSpeechRecognizer exit");
        }

        /// <summary>
        /// Initiate continuous speech recognition from the default microphone.
        /// </summary>
        private async void StartContinuousRecognition()
        {
            Debug.Log("Starting Continuous Speech Recognition.");
            CreateSpeechRecognizer();

            if (recognizer != null)
            {
                Debug.Log("Starting Speech Recognizer.");
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                Debug.Log("Speech Recognizer is now running.");
                isRecording = true;
            }
            Debug.Log("Start Continuous Speech Recognition exit");
        }

        #region Speech Recognition event handlers
        private void SessionStartedHandler(object sender, SessionEventArgs e)
        {
            Debug.Log($"\n    Session started event. Event: {e.ToString()}.");
        }

        private void SessionStoppedHandler(object sender, SessionEventArgs e)
        {
            Debug.Log($"\n    Session event. Event: {e.ToString()}.");
            Debug.Log($"Session Stop detected. Stop the recognition.");
        }

        private void SpeechStartDetectedHandler(object sender, RecognitionEventArgs e)
        {
            Debug.Log($"SpeechStartDetected received: offset: {e.Offset}.");
        }

        private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
        {
            Debug.Log($"SpeechEndDetected received: offset: {e.Offset}.");
            Debug.Log($"Speech end detected.");
        }

        // "Recognizing" events are fired every time we receive interim results during recognition (i.e. hypotheses)
        private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizingSpeech)
            {
                Debug.Log($"HYPOTHESIS: Text={e.Result.Text}");
                if (recognizedString.Length > 1000)
                    recognizedString = string.Empty;

                var text = e.Result.Text.Trim().ToLower();

                var stringinfoNew = new StringInfo(text);
                var stringinfoOld = new StringInfo(recognizedString);

                if (stringinfoNew.LengthInTextElements > stringinfoOld.LengthInTextElements)
                {
                    var idu = new IdeationalUnit();
                    idu.Phrases = new List<Phrase>();
                    string newPart;
                    if (stringinfoOld.LengthInTextElements == 0)
                    {
                        newPart = stringinfoNew.SubstringByTextElements(
                            stringinfoOld.LengthInTextElements, stringinfoNew.LengthInTextElements - stringinfoOld.LengthInTextElements - 1);
                    }
                    else
                    {
                        newPart = stringinfoNew.SubstringByTextElements(
                            stringinfoOld.LengthInTextElements - 1, stringinfoNew.LengthInTextElements - stringinfoOld.LengthInTextElements);
                    }
                    Debug.Log($"HYPOTHESIS: newpart{newPart}");
                    idu.Phrases.Add(new Phrase(newPart,
                        e.Result.OffsetInTicks / 10000000.0f - _lastResultEndTime));
                    _lastResultEndTime = e.Result.OffsetInTicks / 10000000.0f;
                    idu.DetectedTimestamp = _lastResultEndTime;


                    // _latencyTracker.text = (_Timer.ElapsedTime() - _lastResultEndTime).ToString();
                    Debug.Log($"HYPOTHESIS: Timestamp {TimeUtils.GetMSTimestamp()}, Delta {_lastResultEndTime}, text {e.Result.Text}");
                    //TimeUtils.StampCurrentTime("[MSSpeechDetector Test]Get Interim Result " + e.Result.Text);

                    _AvatarBrain.EventSequencer.Push(idu);

                    // enqueue nlp
                    var request = new ParseRequest(stringinfoNew, _lastResultEndTime);
                    _NaturalLanguageParser.HandleInterimText(request);
                }
                lock (threadLocker)
                {
                    recognizedString = $"{text}";
                }
            }
        }

        // "Recognized" events are fired when the utterance end was detected by the server
        private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"RECOGNIZED: Text={e.Result.Text}");
                var request = new ParseRequest(new StringInfo(e.Result.Text),
                e.Result.OffsetInTicks / 10000000.0f);

                //TimeUtils.StampCurrentTime("[MSSpeechDetector Test]Get Final Result ");
                _NaturalLanguageParser.HandleFinalText(request);

                lock (threadLocker)
                {
                    //recognizedString = $"{e.Result.Text}";
                }
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Debug.Log($"NOMATCH: Speech could not be recognized.");
            }
        }

        // "Canceled" events are fired if the server encounters some kind of error.
        // This is often caused by invalid subscription credentials.
        private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            Debug.Log($"CANCELED: Reason={e.Reason}");

            recognizedString = e.ToString();
            if (e.Reason == CancellationReason.Error)
            {
                Debug.LogError($"CANCELED: ErrorDetails={e.ErrorDetails}");
                Debug.LogError("CANCELED: Did you update the subscription info?");
            }
        }
        #endregion

        /// <summary>
        /// Creates a class-level Translation Recognizer for a specific language using Azure credentials
        /// and hooks-up lifecycle & recognition events. Translation can be enabled with one or more target
        /// languages translated simultaneously
        /// </summary>
        void CreateTranslationRecognizer()
        {
            Debug.Log("Creating Translation Recognizer.");

            if (translator == null)
            {
                SpeechTranslationConfig config = SpeechTranslationConfig.FromSubscription(SpeechServiceAPIKey, SpeechServiceRegion);
                config.SpeechRecognitionLanguage = fromLanguage;
                for (int i = 0; i < TranslateToLanguages.Count; i++)
                {
                    config.AddTargetLanguage(ExtractLanguageCode(TranslateToLanguages[i]));
                }
                translator = new TranslationRecognizer(config);

                if (translator != null)
                {
                    translator.Recognizing += RecognizingTranslationHandler;
                    translator.Recognized += RecognizedTranslationHandler;
                    translator.SpeechStartDetected += SpeechStartDetectedHandler;
                    translator.SpeechEndDetected += SpeechEndDetectedHandler;
                    translator.Canceled += CanceledTranslationHandler;
                    translator.SessionStarted += SessionStartedHandler;
                    translator.SessionStopped += SessionStoppedHandler;
                }
            }
            Debug.Log("CreateTranslationRecognizer exit");
        }

        /// <summary>
        /// Extract the language code from the enum used to populate the droplists.
        /// Assumes that an underscore "_" is used as a separator in the enum name.
        /// </summary>
        /// <param name="languageListLabel"></param>
        /// <returns></returns>
        string ExtractLanguageCode(string languageListLabel)
        {
            return languageListLabel.Substring(0, languageListLabel.IndexOf("_"));
        }

        /// <summary>
        /// Initiate continuous speech recognition from the default microphone, including live translation.
        /// </summary>
        private async void StartContinuousTranslation()
        {
            Debug.Log("Starting Continuous Translation Recognition.");
            CreateTranslationRecognizer();

            if (translator != null)
            {
                Debug.Log("Starting Speech Translator.");
                await translator.StartContinuousRecognitionAsync().ConfigureAwait(false);

                Debug.Log("Speech Translator is now running.");
                isRecording = true;
            }
            Debug.Log("Start Continuous Speech Translation exit");
        }

        #region Speech Translation event handlers
        // "Recognizing" events are fired every time we receive interim results during recognition (i.e. hypotheses)
        private void RecognizingTranslationHandler(object sender, TranslationRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.TranslatingSpeech)
            {
                Debug.Log($"RECOGNIZED HYPOTHESIS: Text={e.Result.Text}");
                lock (threadLocker)
                {
                    recognizedString = $"{e.Result.Text}";
                    recognizedString += $"{Environment.NewLine}TRANSLATED HYPOTHESESE:";
                    foreach (var element in e.Result.Translations)
                    {
                        recognizedString += $"{Environment.NewLine}[{element.Key}]: {element.Value}";
                    }
                }
            }
        }

        // "Recognized" events are fired when the utterance end was detected by the server
        private void RecognizedTranslationHandler(object sender, TranslationRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.TranslatedSpeech)
            {
                Debug.Log($"RECOGNIZED: Text={e.Result.Text}");
                lock (threadLocker)
                {
                    recognizedString = $"{e.Result.Text}";
                    recognizedString += $"{Environment.NewLine}TRANSLATED RESULTS:";
                    foreach (var element in e.Result.Translations)
                    {
                        recognizedString += $"{Environment.NewLine}[{element.Key}]: {element.Value}";
                    }
                }
            }
            else if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"RECOGNIZED: Text={e.Result.Text}");
                lock (threadLocker)
                {
                    recognizedString = $"{e.Result.Text}";
                }
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Debug.Log($"NOMATCH: Speech could not be recognized or translated.");
            }
        }

        // "Canceled" events are fired if the server encounters some kind of error.
        // This is often caused by invalid subscription credentials.
        private void CanceledTranslationHandler(object sender, TranslationRecognitionCanceledEventArgs e)
        {
            Debug.Log($"CANCELED: Reason={e.Reason}");

            recognizedString = "";
            if (e.Reason == CancellationReason.Error)
            {
                Debug.LogError($"CANCELED: ErrorDetails={e.ErrorDetails}");
                Debug.LogError($"CANCELED: Did you update the subscription info?");
            }
        }
        #endregion

        void OnDisable()
        {
            StopRecognition();
        }

        /// <summary>
        /// Stops the recognition on the speech recognizer or translator as applicable.
        /// Important: Unhook all events & clean-up resources.
        /// </summary>
        public async void StopRecognition()
        {
            if (recognizer != null)
            {
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                recognizer.Recognizing -= RecognizingHandler;
                recognizer.Recognized -= RecognizedHandler;
                recognizer.SpeechStartDetected -= SpeechStartDetectedHandler;
                recognizer.SpeechEndDetected -= SpeechEndDetectedHandler;
                recognizer.Canceled -= CanceledHandler;
                recognizer.SessionStarted -= SessionStartedHandler;
                recognizer.SessionStopped -= SessionStoppedHandler;
                recognizer.Dispose();
                recognizer = null;
                recognizedString = "";
                Debug.Log("Speech Recognizer is now stopped.");
            }
            if (translator != null)
            {
                await translator.StopContinuousRecognitionAsync().ConfigureAwait(false);
                translator.Recognizing -= RecognizingTranslationHandler;
                translator.Recognized -= RecognizedTranslationHandler;
                translator.SpeechStartDetected -= SpeechStartDetectedHandler;
                translator.SpeechEndDetected -= SpeechEndDetectedHandler;
                translator.Canceled -= CanceledTranslationHandler;
                translator.SessionStarted -= SessionStartedHandler;
                translator.SessionStopped -= SessionStoppedHandler;
                translator.Dispose();
                translator = null;
                recognizedString = "";
                Debug.Log("Speech Translator is now stopped.");
            }
            isRecording = false;
        }

        override public string GetDetectedResult()
		{
			return _resultText.text;
		}
	}
}