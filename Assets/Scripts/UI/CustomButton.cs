using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Tausi.RubiksCube
{
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] CanvasGroup button;
        [SerializeField] UnityEvent onClick;

        float alpha;

        void Awake()
        {
            alpha = .75f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Awake();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            alpha = 1;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }

        void Update()
        {
            button.SetAlpha(alpha, speed: 10);
        }
    }
}