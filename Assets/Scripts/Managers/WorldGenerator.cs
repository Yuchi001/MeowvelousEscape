using System.Collections.Generic;
using Managers.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace World_Generation
{
    [RequireComponent(typeof(Grid))]
    public class WorldGenerator : MonoBehaviour, IManagedSingleton
    {
        public static WorldGenerator Instance { get; private set; }

        [SerializeField] private WorldGenerationSettings settings;
        [SerializeField] private Camera observerCamera;

        private bool _generate = false;

        private Grid _grid;

        private Grid Grid
        {
            get 
            {
                if (_grid == null)
                    _grid = GetComponent<Grid>();
                return _grid; 
            }
        }

        private readonly Dictionary<Vector2Int, WorldChunk> _chunksByCoord = new Dictionary<Vector2Int, WorldChunk>();

        [SerializeField, ReadOnly]
        private readonly List<Vector2Int> _coordsVisibleLastFrame = new List<Vector2Int>();

        [SerializeField]
        private bool randomizeSeedOnStart;

        [SerializeField]
        private int seed;
        private System.Random _rng;

        [Header("Truck")]
        [SerializeField] private int spawnTruckEveryChunk = 200;
        [SerializeField] private WorldChunk truckChunkPrototype;

        public void Init()
        {
            observerCamera = Camera.main;
            _rng = new System.Random(seed);
        }

        public void Generate()
        {
            if (randomizeSeedOnStart)
                seed = Random.Range(int.MinValue, int.MaxValue);
            UpdateChunks();
            _generate = true;
        }

        private void Update()
        {
            if (!_generate) return;
            
            UpdateChunks();
        }

        private void UpdateChunks()
        {
            foreach (var position in _coordsVisibleLastFrame)
                if (_chunksByCoord.TryGetValue(position, out var chunk))
                    if (chunk)
                        chunk.IsVisible = false;

            _coordsVisibleLastFrame.Clear();

            var yExtent = observerCamera.orthographicSize;
            var xExtent = yExtent * observerCamera.aspect;
            var observerPosition = observerCamera.transform.position;
            var bottomLeft = WorldToGrid(new Vector2(
                Mathf.Floor(observerPosition.x - xExtent),
                Mathf.Floor(observerPosition.y - yExtent)));

            var topRight = WorldToGrid(new Vector2(
                Mathf.Ceil(observerPosition.x + xExtent),
                Mathf.Ceil(observerPosition.y + yExtent)));

            for (var j = bottomLeft.y - 1; j <= topRight.y + 1; j++)
            {
                for (var i = bottomLeft.x - 1; i <= topRight.x + 1; i++)
                {
                    var coord = new Vector2Int(i, j);
                    _coordsVisibleLastFrame.Add(coord);
                    if (!_chunksByCoord.TryGetValue(coord, out var chunk))
                    {
                        HandleEmptyCoord(coord);
                        continue;
                    }
                    
                    if (chunk == null) continue;
                    
                    chunk.IsVisible = true;
                }
            }
        }

        private void HandleEmptyCoord(Vector2Int coord)
        {
            if (_chunksByCoord.Count > 100 && _chunksByCoord.Count % spawnTruckEveryChunk == 0)
            {
                var trackChunk = Instantiate(truckChunkPrototype, GridToWorld(coord), Quaternion.identity, transform);
                _chunksByCoord.Add(coord, trackChunk);
                return;
            }
            
            var chunk = (Random.value < settings.SpawnProbability) ? CreateChunk(coord) : null;
            _chunksByCoord.Add(coord, chunk);
        }

        private WorldChunk CreateChunk(Vector2Int coord)
        {
            var chunkPosition = GridToWorld(coord);
            var prototypesList = settings.ChunksPrototypes;
            var randomIndex = _rng.Next(prototypesList.Count);
            var prototype = prototypesList[randomIndex];
            var chunk = Instantiate(prototype, chunkPosition, Quaternion.identity, transform);
            chunk.IsVisible = true;
            chunk.name = $"{prototype.name} {coord}";
            return chunk;
        }

        private Vector2Int WorldToGrid(Vector3 position)
        {
            return (Vector2Int)Grid.WorldToCell(position);
        }

        private Vector2 GridToWorld(Vector2 grid)
        {
            Vector3 localPosition = Grid.CellToLocalInterpolated(grid);
            return transform.TransformPoint(localPosition);
        }
    }
}
