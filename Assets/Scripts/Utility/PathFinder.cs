using BusJamDemo.Grid;
using System.Collections.Generic;
using UnityEngine;

namespace BusJamDemo.Utility
{
    public class PathfindingResult
    {
        public bool PathFound { get; set; }
        public Vector2Int EndCoordinates { get; set; }
        public Dictionary<Vector2Int, Vector2Int> Parents { get; set; }
    }

    public class Pathfinder : MonoBehaviour
    {
        [SerializeField] private GridManager grid;
        public static Pathfinder Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private PathfindingResult FindPathCore(CellPosition startCellPosition)
        {
            var startCoordinates = new Vector2Int(startCellPosition.Row, startCellPosition.Column);
            
            if (!IsMovePossible(startCoordinates.x, startCoordinates.y)) 
            {
                return new PathfindingResult { PathFound = false };
            }

            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> parents = new Dictionary<Vector2Int, Vector2Int>();

            queue.Enqueue(startCoordinates);
            visited.Add(startCoordinates);
            
            int lastRow = grid.RowCount - 1; 
            Vector2Int[] directions =
            {
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 0), new Vector2Int(-1, 0)
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                if (current.x == lastRow && !IsBlocked(current.x, current.y))
                {
                    return new PathfindingResult 
                    { 
                        PathFound = true,
                        EndCoordinates = current,
                        Parents = parents
                    };
                }

                foreach (var dir in directions)
                {
                    Vector2Int neighbor = current + dir;

                    if (neighbor.x < 0 || neighbor.x >= grid.RowCount || neighbor.y < 0 ||
                        neighbor.y >= grid.ColumnCount)
                        continue;

                    if (IsBlocked(neighbor.x, neighbor.y))
                        continue;

                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        parents[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return new PathfindingResult { PathFound = false }; 
        }

        public bool HasAnyPath(CellPosition startCellPosition)
        {
            return FindPathCore(startCellPosition).PathFound;
        }

        public List<CellPosition> GetClosestPathToExit(CellPosition startCellPosition)
        {
            PathfindingResult result = FindPathCore(startCellPosition);
            if (!result.PathFound)
            {
                return null;
            }
            
            var startCoordinates = new Vector2Int(startCellPosition.Row, startCellPosition.Column);
            return ReconstructPath(startCoordinates, result.EndCoordinates, result.Parents);
        }

        private List<CellPosition> ReconstructPath(Vector2Int start, Vector2Int end, Dictionary<Vector2Int, Vector2Int> parents)
        {
            List<CellPosition> path = new List<CellPosition>();
            Vector2Int current = end;

            while (current != start)
            {
                var cellData = grid[current.x, current.y];
                path.Add(cellData.CellPosition);
                
                if (!parents.ContainsKey(current))
                {
                    return null;
                }
                current = parents[current];
            }

            path.Reverse();
            return path;
        }

        private bool IsBlocked(int row, int col)
        {
            var cellData = grid[row, col]; 
            if (cellData.HasItem && cellData.HeldItem is IBlocker)
            {
                return true;
            }
            return false;
        }

        private bool IsMovePossible(int row, int col)
        {
            var cellData = grid[row, col];
            return cellData.HasItem && cellData.HeldItem is Passenger;
        }
    }
}