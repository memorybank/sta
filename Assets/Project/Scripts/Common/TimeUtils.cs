using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;

namespace Playa.Common.Utils
{
    public class TimeUtils
    {
        public static double GetMSTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
    }
}