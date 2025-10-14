using System.Collections.Generic;
using BusJamDemo.Grid;

namespace BusJamDemo.Service
{
    public interface IPathfindingService
    {
        List<CellPosition> FindPath(CellPosition startCellPosition);
        bool HasAnyPath(CellPosition startCellPosition);
    }
}