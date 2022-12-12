using UnityEngine;

namespace Tausi.RubiksCube
{
    [ExecuteAlways]
    public class TransparentUnityWindow : MonoBehaviour
    {
#pragma warning disable CS0414
        [SerializeField] bool noActivate;
        [SerializeField] int targetFrameRate = -1;
#pragma warning restore CS0414

        public void Toggle()
        {
            enabled = !enabled;
        }

#if UNITY_EDITOR
        void OnEnable()
        {
            if (!Application.isPlaying)
                UnityEditor.PlayerSettings.useFlipModelSwapchain = false;
        }
#else
        Color originalBackgroundColor;

        System.IntPtr hWnd;

        readonly GameObject[] windows = new GameObject[50];

        void Awake()
        {
            originalBackgroundColor = Camera.main.backgroundColor;

            hWnd = WindowsAPI.GetActiveWindow();
            WindowsAPI.ExtendFrameIntoClientArea(hWnd);
            WindowsAPI.SetTopMost(hWnd);

            for (int i = 0; i < windows.Length; i++)
            {
                var window = new GameObject();
                window.transform.SetParent(transform);
                window.SetActive(false);

                var collider = window.AddComponent<EdgeCollider2D>();
                collider.usedByEffector = true;

                var effector = window.AddComponent<PlatformEffector2D>();
                effector.useOneWay = true;
                effector.surfaceArc = 180;

                windows[i] = window;
            }
        }

        void Update()
        {
            var hoverAnything = Utils.RaycastHover(Utils.baseLayerMask) || Utils.RaycastHoverUI();
            WindowsAPI.SetTransparent(hWnd, noActivate, clickThrough: !hoverAnything);
        }

        void OnEnable()
        {
            Application.targetFrameRate = targetFrameRate > 0 ? targetFrameRate : -1;

            WindowsAPI.SetTransparent(hWnd, noActivate, clickThrough: true);
            Camera.main.backgroundColor = new();
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
        }

        void OnDisable()
        {
            Application.targetFrameRate = -1;

            WindowsAPI.SetTransparent(hWnd, noActivate, clickThrough: false);
            Camera.main.backgroundColor = originalBackgroundColor;
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }

#endif
    }
}