using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Tausi.RubiksCube
{
    public class Block : MonoBehaviour
    {
        public Cube cube { get; private set; }

        public bool active;
        public Tile[] tiles;

        new Renderer renderer;

        [HideInInspector]
        public float baseBrightness;

        SmoothFloat brightness;
        SmoothFloat scale;

        void Awake()
        {
            renderer = GetComponent<Renderer>();
            baseBrightness = 1;
            brightness = 1;
            scale = 1;
        }

        void Update()
        {
            UpdateHover(out scale.target, out brightness.target);
            transform.localScale = Vector3.one * scale.Update(.05f);

            brightness.Update(.05f);

            renderer.material.SetFloat("_Brightness", brightness);
            foreach (var tile in tiles)
            {
                tile.renderer.material.SetFloat("_Brightness", brightness);
                tile.grayscale.target = active ? 1 : 0;
            }
        }

        void UpdateHover(out float scale, out float brightness)
        {
            scale = 1;
            brightness = baseBrightness;

            if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
                return;

            if (cube.isBusy)
                return;

            if (!Utils.RaycastHover(out var hit))
                return;

            if (hit.transform.parent.TryGetComponent<Tile>(out var tile))
            {
                if (tile.block == this)
                {
                    brightness *= 1.1f;
                    if (tile == CubePlayer.selectedTile)
                        scale *= 1.05f;
                }
                return;
            }
        }

        public void Setup(Cube cube, Vector3Int index)
        {
            this.cube = cube;
            active = true;
            foreach (var tile in tiles)
                tile.Setup(this, index);
        }

        public Tile GetTile(TileLocation location)
        {
            return tiles.FirstOrDefault(x => x.gameObject.activeSelf && x.Is(location));
        }

        public Vector3Int GetIndex()
        {
            return cube.GetIndexByWorldPosition(transform.position);
        }

        public HashSet<Tile> GetActiveTiles()
        {
            return tiles.Where(x => x.gameObject.activeSelf).ToHashSet();
        }

        public int GetRingSortOrder()
        {
            var s = cube.size - 1;
            var i = GetIndex();
            if (i.z == s) return i.x;
            if (0 < i.z && i.z < s && i.x == s) return cube.size + i.z;
            if (i.z == 0) return cube.size + s + (2 - i.x);
            return cube.size * 2 + (s - i.z);
        }
    }
}