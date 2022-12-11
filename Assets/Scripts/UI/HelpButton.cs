using UnityEngine;
using UnityEngine.EventSystems;

namespace Tausi.RubiksCube
{
    public class HelpButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] CanvasGroup button;
        [SerializeField] CanvasGroup info;

        float buttonAlpha;
        float infoAlpha;

        void Awake()
        {
            buttonAlpha = .75f;
            infoAlpha = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Awake();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            buttonAlpha = 1;
            infoAlpha = 1;
        }

        void Update()
        {
            button.SetAlpha(buttonAlpha, speed: 10);
            info.SetAlpha(infoAlpha, speed: 10);
        }
    }
}