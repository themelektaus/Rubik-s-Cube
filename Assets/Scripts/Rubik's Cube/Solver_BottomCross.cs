using System.Linq;

namespace Tausi.RubiksCube
{
    public class Solver_BottomCross : Solver
    {
        protected override void Update()
        {
            base.Update();

            var s = manager.cube.size - 1;
            var c = s / 2;

            var blocks = manager.cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == s && (i.x == c || i.z == c);
            }).ToHashSet();

            foreach (var block in manager.cube.blocks)
            {
                if (!hover)
                {
                    block.active = true;
                    block.baseBrightness = 1;
                    continue;
                }

                if (blocks.Contains(block))
                {
                    block.active = true;
                    block.baseBrightness = 1;
                    continue;
                }

                block.active = false;
                block.baseBrightness = .9f;
            }
        }

        public override void RunPattern(string name)
        {
            var frontside = this.frontside;

            var color = manager.cube.GetBottomTileColor();
            var s = manager.cube.size - 1;
            var c = s / 2;

            var tiles = manager.cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == s && (i.x == c ^ i.z == c);
            })
                .OrderBy(x => x.GetRingSortOrder())
                .Select(x => x.GetTile(TileLocation.Bottom))
                .ToList();

            var colors = tiles.Select(x => x.color).ToList();

            Frontside? newFrontside = null;
            for (int i = 0; i < colors.Count; i++)
            {
                if (colors[i] != color)
                    continue;

                if (colors[i] != colors[(i + 1) % colors.Count])
                    continue;

                newFrontside = (Frontside) i;
                break;
            }

            if (!newFrontside.HasValue)
            {
                for (int i = 0; i < colors.Count; i++)
                {
                    if (colors[i] != color)
                        continue;

                    if (colors[i] != colors[(i + 2) % colors.Count])
                        continue;

                    newFrontside = (Frontside) ((i - 1) % colors.Count);
                    break;
                }
            }

            if (newFrontside.HasValue)
                frontside = newFrontside.Value;

            manager.RunPattern(name, frontside);
        }
    }
}