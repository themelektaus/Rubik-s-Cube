using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Tausi.RubiksCube
{
    [ExecuteAlways]
    public class Cube : MonoBehaviour
    {
        [SerializeField] Block block;
        [SerializeField, Range(0, 2)] float blockDistance = 1.05f;

        [Range(1, 10)] public int size = 3;

        [SerializeField] int currentSeed;
        
        public event Action<Message> onMessage;

        public readonly List<Block> blocks = new();

        int hash;
        
        readonly Queue<IEnumerator> queue = new();
        Coroutine routine;

        public bool isBusy => queue.Count > 0 || routine is not null;

        void Awake()
        {
            blocks.Clear();

            hash = 0;

            queue.Clear();
            if (routine is not null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            var gameObjects = new List<GameObject>();

            foreach (Transform transform in gameObject.transform)
                gameObjects.Add(transform.gameObject);

            foreach (var gameObject in gameObjects)
                Utils.DestroyImmediateInEditor(gameObject);
        }

        void OnDisable()
        {
            Awake();

            transform.localScale = Vector3.one;
        }

        void OnEnable()
        {
            Awake();
        }

        void Update()
        {
            if (routine is not null)
                return;

            int hash = HashCode.Combine(blockDistance, size);
            if (this.hash != hash)
            {
                this.hash = hash;
                Rebuild();
                return;
            }

            TryStartNextRoutine();
        }

        void TryStartNextRoutine()
        {
            if (queue.Count == 0)
                return;

            routine = StartCoroutine(queue.Dequeue());
        }

        void Rebuild()
        {
            foreach (var block in blocks)
                Utils.DestroyImmediateInEditor(block.gameObject);

            blocks.Clear();

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (x > 0 && x < size - 1 && y > 0 && y < size - 1 && z > 0 && z < size - 1)
                            continue;

                        var index = new Vector3Int(x, y, z);

                        var gameObject = Instantiate(this.block.gameObject, transform);

                        var block = gameObject.GetComponent<Block>();
                        block.Setup(this, index);
                        blocks.Add(block);

                        gameObject.transform.localPosition = GetPositionByIndex(index);
                    }
                }
            }

            transform.localScale = Vector3.one * (1f / size);
        }

        public bool TryShuffle(int seed = 0)
        {
            if (isBusy)
                return false;

            Rebuild();
            currentSeed = seed == 0 ? (int) DateTime.Now.Ticks : seed;
            Random.InitState(currentSeed);
            for (int i = 0; i < size * size * size; i++)
            {
                var axis = (Axis) Random.Range(0, 3);
                var index = Random.Range(0, size);
                var clockwise = Random.value > .5f;
                AddSpin(axis, index, clockwise, SpinOptions.Fast);
            }

            return true;
        }

        public void AddSpin(Axis axis, int index, bool clockwise)
        {
            AddSpin(axis, index, clockwise, SpinOptions.Default);
        }

        public void AddSpin(Axis axis, int index, bool clockwise, SpinOptions options)
        {
            if (options.frontside != Frontside.Front && axis != Axis.Y)
            {
                if (axis == Axis.X)
                {
                    if (options.frontside == Frontside.Left || options.frontside == Frontside.Right)
                    {
                        axis = Axis.Z;
                    }
                    if (options.frontside == Frontside.Back || options.frontside == Frontside.Right)
                    {
                        index = size - 1 - index;
                        clockwise = !clockwise;
                    }
                }
                else if (axis == Axis.Z)
                {
                    if (options.frontside == Frontside.Left || options.frontside == Frontside.Right)
                    {
                        axis = Axis.X;
                    }
                    if (options.frontside == Frontside.Left || options.frontside == Frontside.Back)
                    {
                        index = size - 1 - index;
                        clockwise = !clockwise;
                    }
                }
            }

            IEnumerator _()
            {
                var _blocks = blocks.Where(x =>
                {
                    var _index = x.GetIndex();
                    return
                        (axis == Axis.X && _index.x == index) ||
                        (axis == Axis.Y && _index.y == index) ||
                        (axis == Axis.Z && _index.z == index);
                }).ToList();

                if (options.ignoreInactive || _blocks.All(x => x.active))
                {
                    if (options.invokeMessageEvent)
                        onMessage?.Invoke(new () { type = MessageType.Spin, axis = axis, index = index, clockwise = clockwise });

                    var control = SpinControl.Instantiate(this, _blocks);

                    var b = control.GetRotation(axis, clockwise);

                    if (options.speed > 0)
                    {
                        var a = control.holder.localRotation;
                        var v = new Vector3();
                        var t = .06f / options.speed;

                        while (!Utils.Approximately(a.eulerAngles, b.eulerAngles, .1f))
                        {
                            var a_ = a.eulerAngles;
                            var b_ = b.eulerAngles;
                            a = Quaternion.Euler(
                                Mathf.SmoothDampAngle(a_.x, b_.x, ref v.x, t),
                                Mathf.SmoothDampAngle(a_.y, b_.y, ref v.y, t),
                                Mathf.SmoothDampAngle(a_.z, b_.z, ref v.z, t)
                            );
                            control.holder.localRotation = a;
                            yield return null;
                        }
                    }

                    control.holder.localRotation = b;

                    var blocks = control.blocks;
                    control.Destroy();
                    foreach (var block in blocks)
                        block.transform.RoundTo(3);
                }

                routine = null;

                if (options.speed <= 0)
                    TryStartNextRoutine();
            }

            queue.Enqueue(_());
        }

        float GetOffset()
        {
            return size / 2f - .5f;
        }

        Vector3 GetPositionByIndex(Vector3Int index)
        {
            var o = GetOffset();
            var s = size - 1;
            var x = s - index.x;
            var y = s - index.y;
            var z = index.z;
            return new Vector3(x - o, y - o, z - o) * blockDistance;
        }

        public Vector3Int GetIndexByWorldPosition(Vector3 position)
        {
            var o = GetOffset();

            position = transform.InverseTransformPoint(position);
            position /= blockDistance;
            position.x += o;
            position.y += o;
            position.z += o;

            var index = new Vector3Int(
                Mathf.RoundToInt(position.x),
                size - 1 - Mathf.RoundToInt(position.y),
                Mathf.RoundToInt(position.z)
            );

            return index;
        }

        public Block GetBlockByIndex(Vector3Int index)
        {
            return blocks.FirstOrDefault(x => x.GetIndex() == index);
        }

        public TileColor GetBottomTileColor()
        {
            var s = size - 1;
            var index = new Vector3Int(s / 2, s, s / 2);
            return GetBlockByIndex(index).GetTile(TileLocation.Bottom).color;
        }
    }
}