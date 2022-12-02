using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Playa.Common
{
    public static class ThreadsConstants
    {
        public static float GCNLParserCoroutineIntervalTime = 0.04f; // yield return new WaitForSeconds
        public static float GCNLParserRequestTimeout = 1000;         // ms
        public static float WaitForReady = 0.1f; // second

        public static float FacialExpressionPlanningCoroutineIntervalTime = 0.02f; // yield return new WaitForSeconds
    }
}