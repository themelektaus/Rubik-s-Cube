using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Tausi.RubiksCube
{
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] CanvasGroup button;
        [SerializeField] UnityEvent onClick;

        float originalAlpha;
        SmoothCanvasGroup smoothCanvasGroup;

        protected virtual void Awake()
        {
            originalAlpha = button.alpha;
            smoothCanvasGroup = button;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            smoothCanvasGroup.targetAlpha = originalAlpha;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            smoothCanvasGroup.targetAlpha = 1;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }

        protected virtual void Update()
        {
            smoothCanvasGroup.Update(.1f);
        }
    }
}