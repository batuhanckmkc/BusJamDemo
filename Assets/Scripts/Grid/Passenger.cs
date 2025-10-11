using System.Collections.Generic;
using BusJamDemo.Core.Input;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class Passenger : CellItem, IClickable, IBlocker
    {
        public void HandleClick()
        {
            Perform();
        }
        
        public override void Perform()
        {
            var path = Pathfinder.Instance.GetClosestPathToExit(CellData.CellPosition);
            if (path != null && path.Count > 0)
            {
                var itemPlaceData = new ItemPlaceData(this, path[^1]);
                EventManager<ItemPlaceData>.Execute(BoardEvents.OnCellItemPlaced, itemPlaceData);

                MoveAlongPath(path); 
            }
            else
            {
                Debug.Log("No path was found or all exits were occupied.");
            }
        }
        
        private void MoveAlongPath(List<CellPosition> path)
        {
            var followPathSequence = DOTween.Sequence();
            for (int i = 0; i < path.Count; i++)
            {
                followPathSequence.Append(transform.DOMove(path[i].WorldPosition, 0.25f).SetEase(Ease.Linear));
            }
        }
    }
}