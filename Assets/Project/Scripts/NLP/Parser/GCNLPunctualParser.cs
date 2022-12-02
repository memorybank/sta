using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;

using UnityEngine;
using Playa.NLP.Event;
using Google.Cloud.Language.V1;

using Playa.Event;
using Playa.Common;
using Playa.Common.Utils;
using SimpleJSON;
using TMPro;

namespace Playa.NLP.Parser
{
    public class GCNLPunctualParser : NLPResultParser
    {
        private const int _NeedSyntaxRootDetectedOffSet = 3;

        public class PunctuationRequest
        {
            public string NewPart;
            public float StartTimestamp;

            public PunctuationRequest(string newPart, float startTimestamp)
            {
                NewPart = newPart;
                StartTimestamp = startTimestamp;
            }
        }

        public class GCSyntaxDetectedUnit
        {
            public string Content;
            public int LastSyntaxRootDetectedBeginOffSet;
            public string LastRequestString;

        }

        private ConcurrentQueue<PunctuationRequest> _NLParserQueue;          //Queue for text, unrealtime handler

        private int _lastPunctuation = 0;
        private GCSyntaxDetectedUnit _GCSyntaxDetectedUnit;

        private bool _IsRunning;

        private PunctuationUnit punctuationUnit;
        private bool _IsPunctuationUnitNew;

        protected Coroutine _NLParserRoutine;            //coroutine for timed execution

        [SerializeField]private TMP_Text verbText;
        private string detectedText;
        private bool isVerbDetected;

        [SerializeField] private EventSequencer _EventSequencer;

        void Start()
        {
            _GCSyntaxDetectedUnit = new GCSyntaxDetectedUnit();
            _GCSyntaxDetectedUnit.Content = "";
            _GCSyntaxDetectedUnit.LastSyntaxRootDetectedBeginOffSet = 0;
            _GCSyntaxDetectedUnit.LastRequestString = "";
            _NLParserQueue = new ConcurrentQueue<PunctuationRequest>();
            Task.Run(() => TimerCoroutine());

            _EventSequencer.voiceActivityEvent.AddListener(OnVoiceActivityReady);
        }

        void Update()
        {
            if (_IsPunctuationUnitNew)
            {
                ParserSyntaxRootEvent?.Invoke(punctuationUnit);
                _IsPunctuationUnitNew = false;
            }

            if (isVerbDetected)
            {
                verbText.text = "Root£º" + detectedText;
                isVerbDetected = false;
            }
        }

        private void OnDestroy()
        {
            _NLParserQueue.Clear();
            _IsRunning = false;
        }

        public async Task TimerCoroutine()
        {
            _IsRunning = true;
            while (_IsRunning)
            {
                await Task.Delay((int)(ThreadsConstants.GCNLParserCoroutineIntervalTime * 1000));
                if (IsInit)
                {
                    NLRequestDetectVerbPunctuation();
                }
            }
        }

        public void OnVoiceActivityReady(VoiceActivityUnit voiceActivityUnit, int speakerUUID)
        {
            if (IsInit)
            {
                PunctuationRequest request;
                while (_NLParserQueue.TryPeek(out request))
                {
                    if (request.StartTimestamp > voiceActivityUnit.DetectedTimestamp)
                    {
                        break;
                    }

                    _NLParserQueue.TryDequeue(out request);
                }

                _GCSyntaxDetectedUnit.LastSyntaxRootDetectedBeginOffSet = 0;
                _GCSyntaxDetectedUnit.Content = "";
            }
        }

        public void NLRequestDetectVerbPunctuation()
        {
            string content = "";

            PunctuationRequest request;
            float requestTimestamp = 0;
            var e = _NLParserQueue.GetEnumerator();
            while (e.MoveNext())
            {
                request = (PunctuationRequest)e.Current;
                content += request.NewPart;
                requestTimestamp = request.StartTimestamp;
            }

            if (content != "")
            {
                bool b = HasVerbPunctuationDetectedWithTimeout(content, ThreadsConstants.GCNLParserRequestTimeout);

                if (b)
                {
                    punctuationUnit = new PunctuationUnit(false, requestTimestamp);
                    _IsPunctuationUnitNew = true;
                }
            }
        }
        
        private bool MustHasVerbPunctuationDetected(string content)
        {
            Document document = Document.FromPlainText(content);
            AnalyzeSyntaxResponse response = client.AnalyzeSyntax(document);
            foreach (Token t in response.Tokens)
            {
                if (_GCSyntaxDetectedUnit.Content == "")
                {
                    if (t.DependencyEdge.Label == DependencyEdge.Types.Label.Root && t.PartOfSpeech.Tag == PartOfSpeech.Types.Tag.Verb)
                    {
                        detectedText = t.Text.Content;
                        _GCSyntaxDetectedUnit.Content = detectedText;
                        _GCSyntaxDetectedUnit.LastSyntaxRootDetectedBeginOffSet = t.Text.BeginOffset;
                        Debug.Log(string.Format("verbroot detect first root {0} {1}", detectedText, content));
                        isVerbDetected = true;
                        return true;
                    }
                }

                if (t.DependencyEdge.Label == DependencyEdge.Types.Label.Root)
                {
                    if (DetectTokenRoot(t))
                    {
                        Debug.Log(string.Format("verbroot detect root {0} {1}", detectedText, content));
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DetectTokenRoot(Token t)
        {
            if (_GCSyntaxDetectedUnit.Content != "" &&
                _GCSyntaxDetectedUnit.LastSyntaxRootDetectedBeginOffSet + _NeedSyntaxRootDetectedOffSet >= t.Text.BeginOffset)
            {
                return false;
            }

            if (_GCSyntaxDetectedUnit.Content == t.Text.Content)
            {
                return false;
            }

            detectedText = t.Text.Content;
            _GCSyntaxDetectedUnit.Content = detectedText;
            _GCSyntaxDetectedUnit.LastSyntaxRootDetectedBeginOffSet = t.Text.BeginOffset;
            isVerbDetected = true;
            return true;
        }

        private bool HasVerbPunctuationDetectedWithTimeout(string content, double timeout)
        {
            double tstart = TimeUtils.GetMSTimestamp();

            bool b = MustHasVerbPunctuationDetected(content);
            if (b)
            {
                double t = TimeUtils.GetMSTimestamp() - tstart;
                if (t < timeout)
                {
                    Debug.LogWarning(string.Format("verbroot detect first root timeout"));
                    return true;
                }
            }

            return false;
        }

        public override void Parse(ParseRequest request)
        {
            if (request.Info.LengthInTextElements < _lastPunctuation + 1)
            {
                _lastPunctuation = 0;
                return;
            }

            var newPart = request.Info.SubstringByTextElements(
                _lastPunctuation, request.Info.LengthInTextElements - _lastPunctuation - 1);

            newPart.Normalize();
            var a = "£¬".Normalize();
            if (newPart.Contains(a))
            {
                _lastPunctuation += newPart.IndexOf(a) + 1;

                parserCommaPunctuationEvent?.Invoke(new PunctuationUnit(false, request.StartTimestamp));
            }
            //parse verb punctuation
            if (IsInit)
            {
                string newRequestPart = request.Info.String;
                var stringinfoNew = new StringInfo(request.Info.String);
                var stringinfoOld = new StringInfo(_GCSyntaxDetectedUnit.LastRequestString);
                if (stringinfoNew.LengthInTextElements > stringinfoOld.LengthInTextElements)
                {
                    if (stringinfoOld.LengthInTextElements == 0)
                    {
                        newRequestPart = stringinfoNew.SubstringByTextElements(
                            stringinfoOld.LengthInTextElements, stringinfoNew.LengthInTextElements - stringinfoOld.LengthInTextElements - 1);
                    }
                    else
                    {
                        newRequestPart = stringinfoNew.SubstringByTextElements(
                            stringinfoOld.LengthInTextElements - 1, stringinfoNew.LengthInTextElements - stringinfoOld.LengthInTextElements);
                    }
                }

                _NLParserQueue.Enqueue(new PunctuationRequest(newRequestPart, request.StartTimestamp));
                _GCSyntaxDetectedUnit.LastRequestString = request.Info.String;
            }
        }

        public override void ParseFinal(ParseRequest request)
        {
            _lastPunctuation = 0;

            // new sequence

            parserCommaPunctuationEvent?.Invoke(new PunctuationUnit(true, request.StartTimestamp));

        }
    }
}