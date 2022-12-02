using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Google.Cloud.Language.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition;
using Google.Api.Gax;
using Google.Api.Gax.Grpc;

using Playa.Common;
using Playa.Common.Utils;

using Playa.NLP.Event;

namespace Playa.NLP.Parser
{
    public class GCNLParser : NaturalLanguageParser
    {
        private LanguageServiceClient client;

        public Config.GCSRConfig config;

        [SerializeField] private Toggle _NLParserToggle;

        private int _lastPunctuation = 0;

        [SerializeField]private NLPResultParser _punctuationParser;

        [SerializeField]private NLPResultParser _keywordParser;

        [SerializeField]private NLPResultParser _sentimentParser;

        // ui component
        [SerializeField] private TextMeshProUGUI _sentimentText;

        [SerializeField] private TextMeshProUGUI _latencyTracker;

        // Start is called before the first frame update
        public override void Init()
        {
            if (Initialize())
            {
                StartCoroutine(CoClientCheck());

                if (_NLParserToggle.isOn)
                {
                    _punctuationParser.Active = true;
                }

                if (_punctuationParser.Active)
                {
                    _punctuationParser.PassClient(client);
                    _punctuationParser.ParserSyntaxRootEvent.AddListener(OnParserSyntaxRootDetect);
                    _punctuationParser.parserCommaPunctuationEvent.AddListener(OnParserCommaPunctuationDetect);
                }

                if (_keywordParser.Active)
                {
                    _keywordParser.PassClient(client);
                    _keywordParser.parserKeywordEvent.AddListener(OnParserKeywordDetect);
                }

                if (_sentimentParser.Active)
                {
                    _sentimentParser.PassClient(client);
                    _sentimentParser.parserSentimentEvent.AddListener(OnParserSentimentDetect);
                }

                _sentimentText.text = "Neutral";
            }
        }

        private void OnParserSyntaxRootDetect(PunctuationUnit u)
        {
            EventSequencer.Push(u);
        }

        private void OnParserCommaPunctuationDetect(PunctuationUnit u)
        {
            // currently do nothing
        }

        private void OnParserKeywordDetect(KeywordUnit u)
        {
            EventSequencer.Push(u);
        }

        private void OnParserSentimentDetect(SentimentUnit u)
        {
            //update ui
            string color = "red";
            if (u.Score < 0)
            {
                color = "green";
            }
            _sentimentText.text = string.Format("情感识别：唤醒 {0} ,<color={1}>效价 {2}</color> ,类别unruled", u.Magnitude, color, u.Score);

            EventSequencer.Push(u);
        }

        IEnumerator CoClientCheck()
        {
            yield return null;
            Document document = Document.FromPlainText("google language api init");
            try
            {
                AnalyzeSyntaxResponse response = client.AnalyzeSyntax(document);
                IsInit = true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private bool Initialize()
        {
            string credentialJson;

            if (config.googleCredentialLoadFromResources)
            {
                if (string.IsNullOrEmpty(config.googleCredentialFilePath) || string.IsNullOrWhiteSpace(config.googleCredentialFilePath))
                {
                    Debug.LogException(new Exception("The googleCredentialFilePath is empty. Please fill path to file."));
                    return false;
                }

                TextAsset textAsset = Resources.Load<TextAsset>(config.googleCredentialFilePath);

                if (textAsset == null)
                {
                    Debug.LogException(new Exception($"Couldn't load file: {config.googleCredentialFilePath} ."));
                    return false;
                }

                credentialJson = textAsset.text;
            }
            else
            {
                credentialJson = config.googleCredentialJson;
            }

            try
            {
#pragma warning disable CS1701
                //client = LanguageServiceClient.Create();   //使用本地环境变量json配置时放开此注释，段注释下方builder
                LanguageServiceSettings languageServiceSettings = new LanguageServiceSettings()
                {
                    AnalyzeSyntaxSettings = new CallSettings(null, Expiration.FromTimeout(new TimeSpan(0, 0, 2)), null, null, null, null)
                };
                client = new LanguageServiceClientBuilder
                {
                    ChannelCredentials = GoogleCredential.FromJson(credentialJson).ToChannelCredentials(),
                    Settings = languageServiceSettings
                }.Build();

#pragma warning restore CS1701

            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }

            if (client != null)
            {
                return true;
            }

            Debug.LogWarning("checking google language api v1 init fail");
            return false;
        }

        private void OnDestroy()
        {
        }

        void Start()
        {
        }

        public override void HandleInterimText(ParseRequest request)
        {
            if (_punctuationParser.IsInit)
            {
                _punctuationParser.Parse(request);
            }

            if (_keywordParser.IsInit)
            {
                _keywordParser.Parse(request);
                // _latencyTracker.text = (_Timer.ElapsedTime() - request.StartTimestamp).ToString();
            }

            if (_sentimentParser.IsInit)
            {
                _sentimentParser.Parse(request);
            }
        }

        public override void HandleFinalText(ParseRequest request)
        {
            if (_punctuationParser.IsInit)
            {
                _punctuationParser.ParseFinal(request);
            }

            if (_keywordParser.IsInit)
            {
                _keywordParser.ParseFinal(request);
            }

            if (_sentimentParser.IsInit)
            {
                _sentimentParser.ParseFinal(request);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!_NLParserToggle.isOn)
            {
                _punctuationParser.Active = false;
                _punctuationParser.EscapeClient();
            }
            else
            {
                if (_punctuationParser.Active == false)
                {
                    _punctuationParser.Active = true;
                    _punctuationParser.PassClient(client);
                    _punctuationParser.ParserSyntaxRootEvent.AddListener(OnParserSyntaxRootDetect);
                }
            }
        }
    }
}


