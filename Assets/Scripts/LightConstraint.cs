using UnityEngine;

namespace Tausi.RubiksCube
{
    public class LightConstraint : MonoBehaviour
    {
        [SerializeField] Transform source;

        new Light light;
        float range;

        void Awake()
        {
            light = GetComponent<Light>();
            range = light.range;
        }

        void Update()
        {
            light.range = range * source.localScale.x;
        }
    }
}