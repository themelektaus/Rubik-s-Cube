using System.Collections.Generic;

using UnityEngine;

namespace Tausi.RubiksCube
{
    public class SpinControl
    {
        readonly Cube cube;

        public readonly Transform holder;
        public readonly List<Block> blocks;

        public static SpinControl Instantiate(Cube cube, IEnumerable<Block> blocks)
            => new(cube, blocks);

        SpinControl(Cube cube, IEnumerable<Block> blocks)
        {
            this.cube = cube;
            holder = new GameObject().transform;
            holder.SetParent(cube.transform, false);

            this.blocks = new(blocks);
            foreach (var block in blocks)
                block.transform.SetParent(holder);
        }

        public Quaternion GetRotation(Axis axis, bool clockwise)
        {
            var r = 90 * (clockwise ? 1 : -1);
            switch (axis)
            {
                case Axis.X: return Quaternion.Euler(r, 0, 0);
                case Axis.Y: return Quaternion.Euler(0, r, 0);
                case Axis.Z: return Quaternion.Euler(0, 0, r);
            }
            return Quaternion.identity;
        }

        public void Destroy()
        {
            foreach (var block in blocks)
                block.transform.SetParent(cube.transform);

            Utils.DestroyImmediateInEditor(holder.gameObject);
        }
    }
}