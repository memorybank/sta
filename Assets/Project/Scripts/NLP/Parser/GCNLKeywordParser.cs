using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;

using UnityEngine;
using Playa.NLP.Event;
using Google.Cloud.Language.V1;

using Playa.Common;
using Playa.Common.Utils;

namespace Playa.NLP.Parser
{
    public class GCNLKeywordParser : NLPResultParser
    {
        private int _minKeywordLen = 2;

        private int _maxKeywordLen = 5;

        private int _updatedResultLengthInTextElements = 2;

        private StringInfo _lastStringInfo = new StringInfo("");


        public override void Parse(ParseRequest request)
        {
            // Todo
            // Stability seems always be 0.01
            TryKeywords(request);
            _lastStringInfo = request.Info;
        }

        public override void ParseFinal(ParseRequest request)
        {
            // Final result latency is too big
            // TryKeywords(request);
            _lastStringInfo = new StringInfo("");
        }

        private void TryKeywords(ParseRequest request)
        {
            Debug.Log("Keyword event parse full " + request.Info.String);

            if (request.Info.LengthInTextElements < _lastStringInfo.LengthInTextElements)
            {
                return;
            }

            if (request.Info.LengthInTextElements >= _minKeywordLen)
            {
                var newDetectLen = request.Info.LengthInTextElements - _lastStringInfo.LengthInTextElements;
                var pos = _lastStringInfo.LengthInTextElements - 1;
                var extend = 0;
                while (pos >= 0 && extend < _updatedResultLengthInTextElements && 
                    _lastStringInfo.SubstringByTextElements(pos, 1) != request.Info.SubstringByTextElements(pos, 1))
                {
                    newDetectLen++;
                    pos--;
                    extend++;
                }
                var maxNewWordLen = newDetectLen + _maxKeywordLen - 1;
                var minStartPos = Math.Max(request.Info.LengthInTextElements - maxNewWordLen, 0);
                var part = new StringInfo(request.Info.SubstringByTextElements(minStartPos, request.Info.LengthInTextElements - minStartPos));
                Debug.Log("Keyword event parse part " + part.String);

                for (int i = 0; i < newDetectLen; i++)
                {
                    for (int len = _minKeywordLen; len <= Math.Min(_maxKeywordLen, request.Info.LengthInTextElements - i); len++)
                    {
                        var newWord = part.SubstringByTextElements(Math.Max(0, part.LengthInTextElements - len - i)
                            , len);
                        parserKeywordEvent?.Invoke(new KeywordUnit(newWord, request.StartTimestamp));
                    }
                }
            }
        }
    }
}