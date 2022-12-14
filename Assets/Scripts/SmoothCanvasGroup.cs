using UnityEngine;

namespace Tausi.RubiksCube
{
    public class SmoothCanvasGroup
    {
        readonly CanvasGroup canvasGroup;
        readonly SmoothFloat alpha;

        public float targetAlpha { set => alpha.target = value; }

        SmoothCanvasGroup(CanvasGroup canvasGroup)
        {
            this.canvasGroup = canvasGroup;
            alpha = canvasGroup.alpha;
        }

        public static implicit operator SmoothCanvasGroup(CanvasGroup canvasGroup) => new(canvasGroup);

        public void Update(float smoothTime)
        {
            canvasGroup.alpha = alpha.Update(smoothTime);
        }
    }
}