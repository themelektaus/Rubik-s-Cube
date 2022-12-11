using System.Collections.Generic;

using UnityEngine;

namespace Tausi.RubiksCube
{
    public partial class CubePlayer : MonoBehaviour
    {
#if UNITY_EDITOR
        readonly HashSet<History> debugHistories = new();

        void UpdateDebugInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                RunHistory();

            if (Input.GetKeyDown(KeyCode.Alpha0))
                Shuffle();

            if (!patterns)
                return;

            debugHistories.Clear();

            if (Input.GetKeyDown(KeyCode.Alpha1) && patterns.entries.Count > 1)
                debugHistories.Add(patterns.entries[1].history);

            if (Input.GetKeyDown(KeyCode.Alpha2) && patterns.entries.Count > 2)
                debugHistories.Add(patterns.entries[2].history);

            if (Input.GetKeyDown(KeyCode.Alpha3) && patterns.entries.Count > 3)
                debugHistories.Add(patterns.entries[3].history);

            if (Input.GetKeyDown(KeyCode.Alpha4) && patterns.entries.Count > 4)
                debugHistories.Add(patterns.entries[4].history);

            if (Input.GetKeyDown(KeyCode.Alpha5) && patterns.entries.Count > 5)
                debugHistories.Add(patterns.entries[5].history);

            if (Input.GetKeyDown(KeyCode.Alpha6) && patterns.entries.Count > 6)
                debugHistories.Add(patterns.entries[6].history);

            if (Input.GetKeyDown(KeyCode.Alpha7) && patterns.entries.Count > 7)
                debugHistories.Add(patterns.entries[7].history);

            if (Input.GetKeyDown(KeyCode.Alpha8) && patterns.entries.Count > 8)
                debugHistories.Add(patterns.entries[8].history);

            if (Input.GetKeyDown(KeyCode.Alpha9) && patterns.entries.Count > 9)
                debugHistories.Add(patterns.entries[9].history);

            if (debugHistories.Count > 0)
            {
                var frontside = (Frontside) (Mathf.FloorToInt((cube.transform.localEulerAngles.y % 360 + 45) / 90) % 4);
                var skinOptions = SpinOptions.WithFrontside(frontside);
                foreach (var history in debugHistories)
                    history.Run(cube, reverse: false, skinOptions);
            }
        }
#endif
    }
}