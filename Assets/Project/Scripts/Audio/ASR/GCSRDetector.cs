using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using UnityEngine;

using System;
using UnityEngine.UI;

using TMPro;
using Google.Cloud.Speech.V1;
using Playa.Common;
using Playa.Common.Utils;
using Playa.NLP.Parser;
using Playa.Audio.Event;

namespace Playa.Audio.ASR
{
	public class GCSRDetector : AudioDetector
	{
		[SerializeField] private GCSRRecognizer _speechRecognition;

		[SerializeField] private NaturalLanguageParser _NaturalLanguageParser;

		public NaturalLanguageParser NaturalLanguageParser => _NaturalLanguageParser;

		public Dropdown _languageDropdown;

		public float _lastResultEndTime = 0.0f;

		private Playa.Common.Utils.Timer _Timer;

		// UI components
		[SerializeField] private TextMeshProUGUI _resultText;
		[SerializeField] private TextMeshProUGUI _latencyTracker;

		private void Start()
		{
			// _speechRecognition = GCSRRecognizer.Instance;
			_speechRecognition.StreamingRecognitionStartedEvent += StreamingRecognitionStartedEventHandler;
			_speechRecognition.StreamingRecognitionFailedEvent += StreamingRecognitionFailedEventHandler;
			_speechRecognition.StreamingRecognitionEndedEvent += StreamingRecognitionEndedEventHandler;
			_speechRecognition.InterimResultDetectedEvent += InterimResultDetectedEventHandler;
			_speechRecognition.FinalResultDetectedEvent += FinalResultDetectedEventHandler;

			_languageDropdown.ClearOptions();

			for (int i = 0; i < Enum.GetNames(typeof(GCSREnumerators.LanguageCode)).Length; i++)
			{
				_languageDropdown.options.Add(new Dropdown.OptionData(((GCSREnumerators.LanguageCode)i).Parse()));
			}

			_languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == GCSREnumerators.LanguageCode.cmn_Hans_CN.Parse()));

			InitMicrophoneDevices();

			_resultText.text = "Ready";

			_NaturalLanguageParser.Init();
		}

		private void OnDestroy()
		{
			_speechRecognition.StreamingRecognitionStartedEvent -= StreamingRecognitionStartedEventHandler;
			_speechRecognition.StreamingRecognitionFailedEvent -= StreamingRecognitionFailedEventHandler;
			_speechRecognition.StreamingRecognitionEndedEvent -= StreamingRecognitionEndedEventHandler;
			_speechRecognition.InterimResultDetectedEvent -= InterimResultDetectedEventHandler;
			_speechRecognition.FinalResultDetectedEvent -= FinalResultDetectedEventHandler;
		}

		private void Update()
		{
			var spaceReleased = Input.GetKeyUp(KeyCode.Space);
			var touchEnded = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;

			if (!spaceReleased && !touchEnded)
			{
				return;
			}

			if (_speechRecognition.isRecording)
			{
				ClearRecordingData();
			}
			else
			{
				// Async init
				StartCoroutine(InitRecordingData());
			}
		}

		private void InitMicrophoneDevices()
		{
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);
		}

		private IEnumerator InitRecordingData()
		{
			_resultText.text = string.Empty;

			List<List<string>> context = new List<List<string>>();

            yield return _SpeechSource.WaitForClipReady();

			_speechRecognition.UpdateAudioSampleRates(_SpeechSource.SampleRate);

			_Timer = new();
			_Timer.StartTimer();
			_NaturalLanguageParser.StartTimer();

			// _speechRecognition.StartRecordingFromClip(_SpeechSource.Clip);
			_speechRecognition.StartRecordingFromAudioSource(_SpeechSource);

			_speechRecognition.StartStreamingRecognition((GCSREnumerators.LanguageCode)_languageDropdown.value, context);
		}

		private async void ClearRecordingData()
		{
			await _speechRecognition.StopStreamingRecognition();
		}

		private void StreamingRecognitionStartedEventHandler()
		{
			// Do nothing
		}

		private void StreamingRecognitionFailedEventHandler(string error)
		{
			_resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";
		}

		private void StreamingRecognitionEndedEventHandler()
		{
			// Do nothing
		}

		private void InterimResultDetectedEventHandler(StreamingRecognitionResult result)
		{
			if (_resultText.text.Length > 1000)
				_resultText.text = string.Empty;

			var text = result.Alternatives[0].Transcript.Trim().ToLower();

			var stringinfoNew = new StringInfo(text);
			var stringinfoOld = new StringInfo(_resultText.text);

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

				idu.Phrases.Add(new Phrase(newPart,
					(float)result.ResultEndTime.ToTimeSpan().TotalSeconds - _lastResultEndTime));
				_lastResultEndTime = (float)result.ResultEndTime.ToTimeSpan().TotalSeconds;
				idu.DetectedTimestamp = _lastResultEndTime;

				// _latencyTracker.text = (_Timer.ElapsedTime() - _lastResultEndTime).ToString();
				Debug.Log(String.Format("Timestamp {0}, Delta {1}, text {2}", _Timer.ElapsedTime(), _lastResultEndTime, result.Alternatives[0].Transcript));

				_AvatarBrain.EventSequencer.Push(idu);

				// enqueue nlp
				var request = new ParseRequest(stringinfoNew, _lastResultEndTime + _speechRecognition.AccumElapsedStreamingTime);
				_NaturalLanguageParser.HandleInterimText(request);
			}

			_resultText.text = $"{text}";
		}

		private void FinalResultDetectedEventHandler(StreamingRecognitionResult result)
		{
			if (_resultText.text.Length > 1000)
				_resultText.text = string.Empty;

			_resultText.text = "";
			var request = new ParseRequest(new StringInfo(result.Alternatives[0].Transcript),
				(float)result.ResultEndTime.ToTimeSpan().TotalSeconds);

			_NaturalLanguageParser.HandleFinalText(request);
		}

		override public string GetDetectedResult()
        {
			return _resultText.text;
        }
	}
}
