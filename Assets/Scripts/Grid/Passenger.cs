using System.Collections.Generic;
using BusJamDemo.Bus;
using BusJamDemo.Core.Input;
using BusJamDemo.LevelLoad;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class Passenger : CellItem, IClickable, IBlocker
    {
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        public bool CanClick { get; set; } = true;
        public PassengerContent PassengerContent;
        public override void Initialize(CellData cellData, CellContent cellContent)
        {
            PassengerContent = cellContent as PassengerContent;
            skinnedMeshRenderer.material.color = PassengerContent.ColorType.GetColor();
            base.Initialize(cellData, cellContent);
        }

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
                EventManager<ItemPlaceData>.Execute(GameplayEvents.OnCellItemPlaced, itemPlaceData);

                CanClick = false;
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
            followPathSequence.OnComplete(DecidePath);
        }

        private void DecidePath()
        {
            var isGetOn = BusController.Instance.CurrentBus.TryGetOn(this);
            if (isGetOn)
            {
                EventManager<ItemRemoveData>.Execute(GameplayEvents.OnCellItemRemoved, new ItemRemoveData(CellData.CellPosition));
                transform.DOJump(BusController.Instance.CurrentBus.transform.position, 1f, 1, 1f);
            }
            else
            {
                var targetBoardingCell = GridManager.Instance.GetEligibleBoardingCell();
                var itemPlaceData = new ItemPlaceData(this, targetBoardingCell.CellPosition);
                EventManager<ItemPlaceData>.Execute(GameplayEvents.OnCellItemPlaced, itemPlaceData);
                
                transform.DOMove(targetBoardingCell.CellPosition.WorldPosition, 1f);
            }
        }
    }
}