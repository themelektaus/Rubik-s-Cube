using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Tausi.RubiksCube
{
    public static class Utils
    {
        public static void DestroyImmediateInEditor(UnityEngine.Object obj)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
                return;
            }

            Object.DestroyImmediate(obj);
        }

        public static bool RaycastHover(out RaycastHit hit)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, 1000f);
        }

        public static bool RaycastHoverUI(out List<RaycastResult> results)
        {
            results = new List<RaycastResult>();
            var e = EventSystem.current;
            e.RaycastAll(new(e) { position = Input.mousePosition }, results);
            return results.Count > 0;
        }

        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x)
                && Mathf.Approximately(a.y, b.y)
                && Mathf.Approximately(a.z, b.z);
        }

        public static bool Approximately(float a, float b, float threshold)
        {
            return Mathf.Abs(a - b) < threshold;
        }

        public static bool Approximately(Vector3 a, Vector3 b, float threshold)
        {
            return Approximately(a.x, b.x, threshold)
                && Approximately(a.y, b.y, threshold)
                && Approximately(a.z, b.z, threshold);
        }
    }
}