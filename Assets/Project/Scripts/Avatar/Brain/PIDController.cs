using UnityEngine;

namespace Playa.Avatars
{
    public class PIDController : MonoBehaviour
    {
        public enum DerivativeMeasurement
        {
            Velocity,
            ErrorRateOfChange
        }

        public float outPutMin;
        public float outPutMax;

        public float proportionalGain;
        public float integralGain;
        public float derivativeGain;

        private float integrationStored;
        public float integralSaturation;
        public DerivativeMeasurement derivativeMeasurement;

        private float errorLast;
        private float valueLast;

        private bool derivativeInitialized;

        // choose D terms to  use
        private float deriveMeasure = 0;

        public float Power;

        public float CalcSpeed(float dt, float currentValue, float targetValue)
        {
            float error = targetValue - currentValue;

            float P = proportionalGain * error;

            float errorRateOfChange = (error - errorLast) / dt;
            errorLast = error;

            float valueRateOfChange = (currentValue - valueLast) / dt;
            valueLast = currentValue;

            float deriveMeasure;

            if (derivativeInitialized)
            {
                if (derivativeMeasurement == DerivativeMeasurement.Velocity)
                {
                    deriveMeasure = -valueRateOfChange;
                }
                else
                {
                    deriveMeasure = errorRateOfChange;
                }
            }
            else
            {
                derivativeInitialized = true;
            }
            

            float D = derivativeGain * errorRateOfChange;

            // calculate I item
            integrationStored = Mathf.Clamp(integrationStored + (error * dt), -integralSaturation, integralSaturation);
            float I = integralGain * integrationStored;

            float result =  P + I + D;
            return Mathf.Clamp(result, outPutMin, outPutMax);
        }

        public void Reset()
        {
            derivativeInitialized = false;
        }
    }
}

