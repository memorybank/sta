using UnityEngine;

namespace Playa.Common.Utils
{
    public class Timer
    {
        private bool isStart = false;
        private float OldTime = 0f;

        public Timer()
        {
            isStart = false;
            OldTime = Time.time;
        }

        public float ElapsedTime()
        {
            if (isStart)
            {
                return Time.time - OldTime;
            }
            else
            {
                return -1f;
            }
        }

        public void StartTimer()
        {
            if (isStart)
            {
                return;
            }
            OldTime = Time.time;
            isStart = true;
        }

        public void StopTimer()
        {
            if (isStart)
            {
                isStart = false;
            }
        }
        
    }
}

