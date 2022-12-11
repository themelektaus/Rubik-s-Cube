using UnityEngine;

namespace Tausi.RubiksCube
{
    public class Tile : MonoBehaviour
    {
        public TileColor color;
        
        public Block block { get; private set; }

        public new Collider collider { get; private set; }
        public new Renderer renderer { get; private set; }
        public Color rendererColor { get; private set; }

        public SmoothFloat grayscale = 1;

        Vector3 direction => block.cube.transform.InverseTransformDirection(transform.up);

        void Awake()
        {
            collider = GetComponentInChildren<Collider>();
            renderer = GetComponentInChildren<Renderer>();
            rendererColor = renderer.material.color;
        }

        void Update()
        {
            grayscale.Update(.1f);
            var color = rendererColor;
            color.r = Mathf.Lerp(.4f, color.r, grayscale);
            color.g = Mathf.Lerp(.4f, color.g, grayscale);
            color.b = Mathf.Lerp(.4f, color.b, grayscale);
            renderer.material.color = color;
        }

        public bool Is(TileLocation location) => location switch
        {
            TileLocation.Top => Utils.Approximately(direction, Vector3.up, .1f),
            TileLocation.Front => Utils.Approximately(direction, -Vector3.forward, .1f),
            TileLocation.Back => Utils.Approximately(direction, Vector3.forward, .1f),
            TileLocation.Left => Utils.Approximately(direction, -Vector3.left, .1f),
            TileLocation.Right => Utils.Approximately(direction, -Vector3.right, .1f),
            TileLocation.Bottom => Utils.Approximately(direction, Vector3.down, .1f),
            _ => false
        };

        public void Setup(Block block, Vector3Int index)
        {
            this.block = block;
            gameObject.SetActive(color switch
            {
                TileColor.Red => index.z == 0,
                TileColor.Blue => index.x == 0,
                TileColor.Green => index.x == block.cube.size - 1,
                TileColor.Orange => index.z == block.cube.size - 1,
                TileColor.Yellow => index.y == block.cube.size - 1,
                _ => index.y == 0,
            });
        }

    }
}