using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Tausi.RubiksCube
{
    public class Solver : MonoBehaviour
    {
        public Frontside frontside;

        [SerializeField] List<Transform> arrows;
        [SerializeField] UnityEvent onClick;
        
        protected SolverManager manager;
        new Collider collider;
        Renderer colliderRenderer;

        readonly Dictionary<Transform, (Renderer[] renderers, Vector3 scale)> arrowInfos = new();

        protected bool hover;

        float alpha;
        bool down;

        void Awake()
        {
            manager = GetComponentInParent<SolverManager>();
            collider = GetComponentInChildren<Collider>();
            colliderRenderer = collider.GetComponentInChildren<Renderer>();

            foreach (var arrow in arrows)
                arrowInfos.Add(arrow, (arrow.GetComponentsInChildren<Renderer>(), arrow.localScale));
        }

        void OnEnable()
        {
            alpha = 0;
            down = false;
            collider.enabled = true;
        }

        void OnDisable()
        {
            alpha = 0;
            down = false;
            collider.enabled = false;

            colliderRenderer.SetAlpha(0);

            foreach (var (renderers, _) in arrowInfos.Values)
                foreach (var renderer in renderers)
                    renderer.SetAlpha(0);
        }

        protected virtual void Update()
        {
            hover = false;

            if (manager.cube.isBusy)
            {
                down = false;
            }
            else
            {
                if (!Input.GetMouseButton(1) && !Input.GetMouseButton(2))
                    if (Utils.RaycastHover(out var hit))
                        if (hit.collider == collider)
                            hover = true;

                if (Input.GetMouseButtonUp(0))
                {
                    if (down && hover)
                        onClick.Invoke();

                    down = false;
                }

                if (hover)
                    if (Input.GetMouseButtonDown(0))
                        down = true;
            }

            alpha = Mathf.Clamp(alpha + Time.deltaTime * 10 * (hover ? 1 : -1), 0, 1);

            colliderRenderer.SetAlpha(manager.cube.isBusy ? 0 : (1 - alpha) * .4901961f);

            foreach (var arrowInfo in arrowInfos)
            {
                foreach (var renderer in arrowInfo.Value.renderers)
                    renderer.SetAlpha(alpha);

                arrowInfo.Key.transform.localScale = Vector3.Lerp(arrowInfo.Value.scale * .9f, arrowInfo.Value.scale, alpha);
            }
        }

        public virtual void RunPattern(string name)
        {
            manager.RunPattern(name, frontside);
        }
    }
}