using UnityEngine;
using UnityEngine.EventSystems;

namespace Tausi.RubiksCube
{
    public class HelpButton : CustomButton
    {
        [SerializeField] CanvasGroup info;

        SmoothCanvasGroup infoCanvasGroup;

        protected override void Awake()
        {
            base.Awake();
            infoCanvasGroup = info;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            infoCanvasGroup.targetAlpha = 0;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            infoCanvasGroup.targetAlpha = .95f;
        }

        protected override void Update()
        {
            base.Update();
            infoCanvasGroup.Update(.1f);
        }
    }
}