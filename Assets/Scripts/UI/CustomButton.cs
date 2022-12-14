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
        float alpha;

        void Awake()
        {
            originalAlpha = button.alpha;
            FadeOut();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            FadeOut();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            FadeIn();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
        
        void FadeOut()
        {
            alpha = originalAlpha;
        }
        
        void FadeIn()
        {
            alpha = 1;
        }

        void Update()
        {
            button.SetAlpha(alpha, speed: 10);
        }
    }
}