using UnityEngine;
using UnityEngine.UI;

namespace Tausi.RubiksCube
{
    public class CubeWapper : MonoBehaviour
    {
        [SerializeField] Cube cube;
        [SerializeField] RectTransform ui;

        CanvasScaler canvasScaler;
        Vector2 size;

        public bool flipped;

        Vector3 lastMousePosition;

        readonly SmoothVector3 rotation = new Vector3(-26, 26, 0);

        SmoothFloat scale;

        Vector3 mouseDragOffset;

        void Awake()
        {
            if (ui)
            {
                canvasScaler = ui.GetComponentInParent<CanvasScaler>();
                size = ui.sizeDelta;
            }

            scale = transform.localScale.x;
            scale.target = .5f;
        }

        void Update()
        {
            UpdateUI();

            if (Utils.RaycastHover(out _))
            {
                var mouseWheel = Input.mouseScrollDelta.y;
                if (mouseWheel != 0)
                    scale.target += (mouseWheel > 0 ? 1 : -1) / 10f * scale.target;
                scale.target = Mathf.Clamp(scale.target, .15f, 1.4f);
            }

            transform.localScale = Vector3.one * scale.Update(.05f);

            var mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;

            if (Input.GetMouseButtonDown(2))
                mouseDragOffset = mousePosition - Camera.main.WorldToScreenPoint(transform.position);

            if (Input.GetMouseButton(2))
            {
                var mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition - mouseDragOffset);
                var position = transform.position;
                position.x = mouseWorldPosition.x;
                position.y = mouseWorldPosition.y;
                transform.position = position;
            }

            if (Input.GetMouseButton(1))
            {
                var delta = mousePosition - lastMousePosition;
                if (!Utils.Approximately(delta, Vector3.zero))
                {
                    rotation.target.x = Mathf.Clamp(rotation.target.x + delta.y / 4, -60, 60);
                    rotation.target.x = Mathf.Clamp(rotation.target.x, -60, 60);
                    rotation.target.y += (flipped ? 1 : -1) * delta.x / 4;
                }
            }

            lastMousePosition = mousePosition;

            rotation.target.z = flipped ? 180 : 0;
            rotation.Update(.1f);

            Vector3 eulerAngles;

            eulerAngles = transform.localEulerAngles;
            eulerAngles.x = rotation.current.x;
            eulerAngles.z = rotation.current.z;
            transform.localEulerAngles = eulerAngles;

            eulerAngles = cube.transform.localEulerAngles;
            eulerAngles.y = rotation.current.y;
            cube.transform.localEulerAngles = eulerAngles;
        }

        void UpdateUI()
        {
            if (!ui) return;
            var r = canvasScaler ? canvasScaler.referenceResolution : new(Screen.width, Screen.height);
            var v = Camera.main.WorldToViewportPoint(transform.position);
            var s = transform.localScale.x;
            ui.anchoredPosition = new Vector2(r.x * v.x, r.y * v.y);
            ui.sizeDelta = size * (s >= 1 ? s : Mathf.Lerp(.25f, 1, s));
        }
    }
}