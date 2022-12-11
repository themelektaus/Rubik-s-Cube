using UnityEngine;

namespace Tausi.RubiksCube
{
    [RequireComponent(typeof(Cube))]
    public partial class CubePlayer : MonoBehaviour
    {
        public static Tile selectedTile;

        public struct BottomCornerTwistLock
        {
            public Solver solver;
            public int step;
        }
        public BottomCornerTwistLock bottomCornorTwistLock;

        public HistoryCollection patterns;

        [SerializeField] History history = new();

        public Cube cube { get; private set; }

        void Awake()
        {
            cube = GetComponent<Cube>();
            cube.onMessage += OnMessage;
        }

        void Start()
        {
            RunHistory();
        }

        void Update()
        {
            UpdateMouseInput();
#if UNITY_EDITOR
            UpdateDebugInput();
#endif
        }

        void OnDestroy()
        {
            cube.onMessage -= OnMessage;
        }

        void OnMessage(Message message)
        {
            if (message.type == MessageType.Spin)
            {
                history.AddSpin(message.axis, message.index, message.clockwise);
                return;
            }
        }

        void UpdateMouseInput()
        {
            if (cube.isBusy)
            {
                selectedTile = null;
                return;
            }

            if (!Utils.RaycastHover(out var hit))
                return;

            if (!hit.transform.parent.TryGetComponent(out Tile tile))
                return;

            if (Input.GetMouseButtonDown(0))
                selectedTile = tile;

            if (!selectedTile)
                return;

            if (Input.GetMouseButton(0))
            {
                if (UpdateSpin(tile))
                    selectedTile = null;
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectedTile = null;
            }
        }

        bool Spin(Axis axis, int index, bool clockwise)
        {
            cube.AddSpin(axis, index, clockwise);
            return true;
        }

        bool UpdateSpin(Tile tile)
        {
            if (selectedTile.block == tile.block)
            {
                if (
                    (selectedTile.Is(TileLocation.Top) && tile.Is(TileLocation.Back)) ||
                    (selectedTile.Is(TileLocation.Front) && tile.Is(TileLocation.Top)) ||
                    (selectedTile.Is(TileLocation.Bottom) && tile.Is(TileLocation.Front)) ||
                    (selectedTile.Is(TileLocation.Back) && tile.Is(TileLocation.Bottom))
                )
                    if (Spin(Axis.X, tile.block.GetIndex().x, true))
                        return true;

                if (
                    (selectedTile.Is(TileLocation.Back) && tile.Is(TileLocation.Top)) ||
                    (selectedTile.Is(TileLocation.Top) && tile.Is(TileLocation.Front)) ||
                    (selectedTile.Is(TileLocation.Bottom) && tile.Is(TileLocation.Back)) ||
                    (selectedTile.Is(TileLocation.Front) && tile.Is(TileLocation.Bottom))
                )
                    if (Spin(Axis.X, tile.block.GetIndex().x, false))
                        return true;

                if (
                    (selectedTile.Is(TileLocation.Top) && tile.Is(TileLocation.Right)) ||
                    (selectedTile.Is(TileLocation.Left) && tile.Is(TileLocation.Top)) ||
                    (selectedTile.Is(TileLocation.Bottom) && tile.Is(TileLocation.Left)) ||
                    (selectedTile.Is(TileLocation.Right) && tile.Is(TileLocation.Bottom))
                )
                    if (Spin(Axis.Z, tile.block.GetIndex().z, true))
                        return true;

                if (
                    (selectedTile.Is(TileLocation.Right) && tile.Is(TileLocation.Top)) ||
                    (selectedTile.Is(TileLocation.Top) && tile.Is(TileLocation.Left)) ||
                    (selectedTile.Is(TileLocation.Left) && tile.Is(TileLocation.Bottom)) ||
                    (selectedTile.Is(TileLocation.Bottom) && tile.Is(TileLocation.Right))
                )
                    if (Spin(Axis.Z, tile.block.GetIndex().z, false))
                        return true;

                if (
                    (selectedTile.Is(TileLocation.Front) && tile.Is(TileLocation.Right)) ||
                    (selectedTile.Is(TileLocation.Left) && tile.Is(TileLocation.Front)) ||
                    (selectedTile.Is(TileLocation.Back) && tile.Is(TileLocation.Left)) ||
                    (selectedTile.Is(TileLocation.Right) && tile.Is(TileLocation.Back))
                )
                    if (Spin(Axis.Y, tile.block.GetIndex().y, true))
                        return true;

                if (
                    (selectedTile.Is(TileLocation.Right) && tile.Is(TileLocation.Front)) ||
                    (selectedTile.Is(TileLocation.Front) && tile.Is(TileLocation.Left)) ||
                    (selectedTile.Is(TileLocation.Left) && tile.Is(TileLocation.Back)) ||
                    (selectedTile.Is(TileLocation.Back) && tile.Is(TileLocation.Right))
                )
                    if (Spin(Axis.Y, tile.block.GetIndex().y, false))
                        return true;
            }
            else
            {
                var a = selectedTile.block.GetIndex();
                var b = tile.block.GetIndex();

                bool top = selectedTile.Is(TileLocation.Top) && tile.Is(TileLocation.Top);
                bool bottom = selectedTile.Is(TileLocation.Bottom) && tile.Is(TileLocation.Bottom);
                bool front = selectedTile.Is(TileLocation.Front) && tile.Is(TileLocation.Front);
                bool back = selectedTile.Is(TileLocation.Back) && tile.Is(TileLocation.Back);
                bool left = selectedTile.Is(TileLocation.Left) && tile.Is(TileLocation.Left);
                bool right = selectedTile.Is(TileLocation.Right) && tile.Is(TileLocation.Right);

                if (top || bottom)
                {
                    if (a.x == b.x)
                    {
                        if (a.z + 1 == b.z)
                            if (Spin(Axis.X, a.x, top))
                                return true;

                        if (a.z - 1 == b.z)
                            if (Spin(Axis.X, a.x, bottom))
                                return true;
                    }
                    else if (a.z == b.z)
                    {
                        if (a.x + 1 == b.x)
                        {
                            if (Spin(Axis.Z, a.z, bottom))
                                return true;
                        }

                        if (a.x - 1 == b.x)
                        {
                            if (Spin(Axis.Z, a.z, top))
                                return true;
                        }
                    }
                }

                if (front || back)
                {
                    if (a.x == b.x)
                    {
                        if (a.y + 1 == b.y)
                        {
                            if (Spin(Axis.X, a.x, back))
                                return true;
                        }

                        if (a.y - 1 == b.y)
                        {
                            if (Spin(Axis.X, a.x, front))
                                return true;
                        }
                    }
                    else if (a.y == b.y)
                    {
                        if (a.x + 1 == b.x)
                        {
                            if (Spin(Axis.Y, a.y, back))
                                return true;
                        }

                        if (a.x - 1 == b.x)
                        {
                            if (Spin(Axis.Y, a.y, front))
                                return true;
                        }
                    }
                }

                if (left || right)
                {
                    if (a.y == b.y)
                    {
                        if (a.z + 1 == b.z)
                        {
                            if (Spin(Axis.Y, a.y, right))
                                return true;
                        }

                        if (a.z - 1 == b.z)
                        {
                            if (Spin(Axis.Y, a.y, left))
                                return true;
                        }
                    }
                    else if (a.z == b.z)
                    {
                        if (a.y + 1 == b.y)
                        {
                            if (Spin(Axis.Z, a.z, right))
                                return true;
                        }

                        if (a.y - 1 == b.y)
                        {
                            if (Spin(Axis.Z, a.z, left))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public void ClearHistory()
        {
            history.messages.Clear();
        }

        public void RunHistory()
        {
            history.Run(cube, reverse: false, SpinOptions.InstantAndWithoutMessageEvent);
        }

        public void Shuffle()
        {
            ClearHistory();
            cube.TryShuffle();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}