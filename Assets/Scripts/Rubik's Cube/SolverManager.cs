using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tausi.RubiksCube
{
    public class SolverManager : MonoBehaviour
    {
        static readonly TileLocation[] sideTileLocations = new[]
        {
            TileLocation.Front,
            TileLocation.Left,
            TileLocation.Right,
            TileLocation.Back
        };

        [SerializeField] CubeWapper cubeWrapper;
        [SerializeField] CubePlayer cubePlayer;
        [SerializeField] SolveLevel solveLevel;

        [SerializeField] Solver[] middleSolvers;
        [SerializeField] Solver[] bottomCrossSolvers;
        [SerializeField] Solver[] bottomCrossRingSolvers;
        [SerializeField] Solver[] bottomCornerSolvers;
        [SerializeField] Solver[] bottomCornerTurnSolvers;

        SolveLevel lastSolveLevel;

        public Cube cube => cubePlayer.cube;

        void Update()
        {
            bool finalLock = cubePlayer.bottomCornorTwistLock.step > 0;

            if (!cube.isBusy)
            {
                if (cubeWrapper.flipped)
                    if (!finalLock && (int) solveLevel < (int) SolveLevel.Middle)
                        cubeWrapper.flipped = false;

                solveLevel = CalcSolveLevel();

                if (lastSolveLevel != solveLevel)
                {
                    var levelUp = lastSolveLevel < solveLevel;
                    lastSolveLevel = solveLevel;

                    if (levelUp)
                    {
                        if (cubeWrapper.flipped)
                        {
                            if ((int) solveLevel >= (int) SolveLevel.Completed)
                                cubeWrapper.flipped = false;
                        }
                        else
                        {
                            if ((int) solveLevel < (int) SolveLevel.Completed && (int) solveLevel >= (int) SolveLevel.Middle)
                                cubeWrapper.flipped = true;
                        }
                    }
                }
            }

            foreach (var solver in middleSolvers)
                solver.enabled = !finalLock && solveLevel == SolveLevel.TopRingT;

            foreach (var solver in bottomCrossSolvers)
                solver.enabled = !finalLock && solveLevel == SolveLevel.Middle;

            foreach (var solver in bottomCrossRingSolvers)
                solver.enabled = !finalLock && solveLevel == SolveLevel.BottomCross;

            foreach (var solver in bottomCornerSolvers)
                solver.enabled = !finalLock && solveLevel == SolveLevel.BottomCrossRing;

            foreach (var solver in bottomCornerTurnSolvers)
            {
                if (finalLock)
                    solver.enabled = solver == cubePlayer.bottomCornorTwistLock.solver;
                else
                    solver.enabled = solveLevel == SolveLevel.BottomCornersTwisted;
            }

            if (solveLevel == SolveLevel.BottomCornersTwisted)
                foreach (var block in cube.blocks)
                    block.active = cube.isBusy || !finalLock || block.GetIndex().y == 2;
        }

        public SolveLevel CalcSolveLevel()
        {
            if (cubePlayer.bottomCornorTwistLock.step > 0)
                return SolveLevel.BottomCornersTwisted;

            // Return if the top of the cube is not solved
            if (!IsTopSolved())
                return SolveLevel.Shuffled;

            // Return if the cube is not ready for solving the middle part
            if (!IsTopRingSolved())
                return SolveLevel.Top;

            // Return if the middle part is not solved
            if (!IsMiddleSolved())
                return SolveLevel.TopRingT;

            // Return if there is no bottom cross
            if (!HasBottomCross())
                return SolveLevel.Middle;

            // Return if the bottom cross has not the correct sides
            int refIndex = HasCorrectSidesAtBottomCross();
            if (refIndex == -1)
                return SolveLevel.BottomCross;

            // Return if any bottom corner is incorrect (independent of their orientations)
            if (!AreBottomCornersCorrect(refIndex))
                return SolveLevel.BottomCrossRing;

            // Return if the cube is not solved (kinda)
            if (!IsCubeSolved(exact: false))
                return SolveLevel.BottomCornersTwisted;

            // Return if the cube is not solved
            if (!IsCubeSolved(exact: true))
                return SolveLevel.AlmostCompleted;

            // Now the cube is completely solved
            return SolveLevel.Completed;
        }

        bool IsTopSolved()
        {
            return HasSameColor(
                cube.blocks
                    .Where(x => x.GetIndex().y == 0)
                    .Select(x => x.GetTile(TileLocation.Top))
            );
        }

        bool IsTopRingSolved()
        {
            var s = cube.size - 1;
            var c = s / 2;

            var blocks = cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                if (i.y == 0) return true;
                if (i.y == 1) return i.x == c || i.z == c;
                return false;
            }).ToHashSet();

            foreach (var tileLocation in sideTileLocations)
            {
                var tiles = blocks.Select(x => x.GetTile(tileLocation)).Where(x => x).ToHashSet();
                if (tiles.Count != 4 || !HasSameColor(tiles))
                    return false;
            }

            return true;
        }

        bool IsMiddleSolved()
        {
            var s = cube.size - 1;
            var c = s / 2;

            var blocks = cube.blocks.Where(x => x.GetIndex().y == c).ToHashSet();

            foreach (var tileLocation in sideTileLocations)
            {
                var tiles = blocks.Select(x => x.GetTile(tileLocation)).Where(x => x).ToHashSet();
                if (tiles.Count != 3 || !HasSameColor(tiles))
                    return false;
            }

            return true;
        }

        bool HasBottomCross()
        {
            var s = cube.size - 1;
            var c = s / 2;

            var blocks = cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == s && (i.x == c || i.z == c);
            });

            return HasSameColor(blocks.Select(x => x.GetTile(TileLocation.Bottom)));
        }

        int HasCorrectSidesAtBottomCross()
        {
            var color = cube.GetBottomTileColor();
            var s = cube.size - 1;
            var c = s / 2;

            var blocks = cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == s && (i.x == c ^ i.z == c);
            }).OrderBy(x => x.GetRingSortOrder()).ToList();

            var refBlocks = cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == c && (i.x == c ^ i.z == c);
            }).OrderBy(x => x.GetRingSortOrder()).ToList();

            for (int i = 0; i < 4; i++)
            {
                var counter = 0;
                for (int j = 0; j < blocks.Count; j++)
                {
                    var block = blocks[j];
                    var tile = block.GetActiveTiles().FirstOrDefault(x => x.color != color);

                    var refBlock = refBlocks[(j + i) % 4];
                    var refTile = refBlock.GetActiveTiles().FirstOrDefault();

                    if (tile.color == refTile.color)
                        counter++;
                }
                if (counter == 4)
                    return i;
            }

            return -1;
        }

        bool AreBottomCornersCorrect(int refIndex)
        {
            var color = cube.GetBottomTileColor();
            var s = cube.size - 1;
            var c = s / 2;

            var blocks = cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == s && i.x != c && (i.x % s == i.z % s);
            }).OrderBy(x => x.GetRingSortOrder()).ToList();

            var refBlocks = cube.blocks.Where(x =>
            {
                var i = x.GetIndex();
                return i.y == s - 1 && i.x != c && (i.x % s == i.z % s);
            }).OrderBy(x => x.GetRingSortOrder()).ToList();

            for (int j = 0; j < blocks.Count; j++)
            {
                var block = blocks[j];
                var tiles = block.GetActiveTiles();

                var refBlock = refBlocks[(j + refIndex) % 4];
                var refTiles = refBlock.GetActiveTiles().Where(x => x.color != color).ToHashSet();

                foreach (var refTile in refTiles)
                    if (!tiles.Any(z => z.color == refTile.color))
                        return false;
            }

            return true;
        }

        bool IsCubeSolved(bool exact)
        {
            var s = cube.size - 1;

            var blocks = cube.blocks
                .Where(x => x.GetIndex().y == s)
                .ToHashSet();

            if (!HasSameColor(blocks.Select(x => x.GetTile(TileLocation.Bottom))))
                return false;

            foreach (var tileLocation in sideTileLocations)
            {
                var tiles = blocks.Select(x => x.GetTile(tileLocation)).Where(x => x).ToHashSet();
                if (tiles.Count != 3 || !HasSameColor(tiles))
                    return false;
            }

            return !exact || HasSameColor(
                cube.blocks
                    .Where(x => x.GetIndex().z == 0)
                    .Select(x => x.GetTile(TileLocation.Front))
            );
        }

        bool HasSameColor(IEnumerable<Tile> tiles)
        {
            TileColor? targetTileColor = null;
            foreach (var tile in tiles)
            {
                if (!tile)
                    return false;

                if (!targetTileColor.HasValue)
                {
                    targetTileColor = tile.color;
                    continue;
                }

                if (targetTileColor != tile.color)
                    return false;
            }
            return true;
        }

        public void RunPattern(string name, Frontside frontside)
        {
            var pattern = cubePlayer.patterns.entries.FirstOrDefault(x => x.name == name);
            if (pattern is null)
                return;

            pattern.history.Run(cube, reverse: false, SpinOptions.WithFrontside(frontside));
        }

        public void PerformBottomCornerTwist(Solver solver)
        {
            var cornerLock = cubePlayer.bottomCornorTwistLock;
            cornerLock.solver = solver;
            cornerLock.step--;
            while (cornerLock.step < 0)
                cornerLock.step += 3;
            cubePlayer.bottomCornorTwistLock = cornerLock;
        }
    }
}