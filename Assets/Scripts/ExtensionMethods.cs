using System;

using UnityEngine;

namespace Tausi.RubiksCube
{
    public static class ExtendsionMethods
    {
        public static void Times(this int @this, Action callback)
        {
            for (int i = 0; i < @this; i++)
                callback();
        }

        public static void SetAlpha(this Renderer @this, float alpha)
        {
            var color = @this.material.color;
            color.a = alpha;
            @this.material.color = color;
        }

        public static void SetAlpha(this CanvasGroup @this, float alpha, float speed)
        {
            if (@this.alpha < alpha)
                @this.alpha = Mathf.Min(alpha, @this.alpha + Time.deltaTime * speed);
            else if (@this.alpha > alpha)
                @this.alpha = Mathf.Max(alpha, @this.alpha - Time.deltaTime * speed);
        }

        public static int Pow(this int @this, int p)
        {
            if (p < 0) throw new Exception("p less than zero is not supported");
            if (p == 0) return 0;
            int result = @this;
            for (int i = 1; i < p; i++)
                result *= @this;
            return result;
        }

        public static Vector3 RoundTo(this Vector3 @this, int decimals)
        {
            var power = Pow(10, decimals);
            @this.x = Mathf.Round(@this.x * power) / power;
            @this.y = Mathf.Round(@this.y * power) / power;
            @this.z = Mathf.Round(@this.z * power) / power;
            return @this;
        }

        public static void RoundTo(this Transform @this, int decimals)
        {
            @this.localPosition = RoundTo(@this.localPosition, decimals);
            @this.localEulerAngles = RoundTo(@this.localEulerAngles, decimals);
            @this.localScale = RoundTo(@this.localScale, decimals);
        }

    }
}