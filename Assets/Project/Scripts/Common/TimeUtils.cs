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

        public static void StampCurrentTime(string name)
        {
            DateTime dt1 = DateTime.Now;
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Debug.Log(name + " Current date time is: " + dt1.ToString() + " timestamp " + timestamp.ToString());
        }
    }
}