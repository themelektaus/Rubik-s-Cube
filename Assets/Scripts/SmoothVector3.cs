using UnityEngine;

namespace Tausi.RubiksCube
{
    public class SmoothVector3
    {
        public Vector3 target;
        public Vector3 current { get; private set; }
        Vector3 currentVelocity;

        SmoothVector3(Vector3 value)
        {
            target = value;
            current = value;
        }

        public static implicit operator SmoothVector3(Vector3 value) => new(value);

        public static implicit operator Vector3(SmoothVector3 value) => value.current;

        public Vector3 Update(float smoothTime)
        {
            return current = Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);
        }
    }
}