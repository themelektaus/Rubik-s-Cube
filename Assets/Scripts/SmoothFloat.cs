using UnityEngine;

namespace Tausi.RubiksCube
{
    public class SmoothFloat
    {
        public float target;
        public float current { get; private set; }
        float currentVelocity;

        SmoothFloat(float value)
        {
            target = value;
            current = value;
        }

        public static implicit operator SmoothFloat(float value) => new(value);

        public static implicit operator float(SmoothFloat value) => value.current;

        public float Update(float smoothTime)
        {
            return current = Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime);
        }
    }
}