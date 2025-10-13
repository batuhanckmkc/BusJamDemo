using System.Collections.Generic;
using BusJamDemo.Bus;
using BusJamDemo.Core.Input;
using BusJamDemo.LevelLoad;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public enum PassengerGameState { GridState, BoardingState, BusState }
    public enum PassengerAnimationState { Idle, Run }
    public class Passenger : CellItem, IClickable, IBlocker
    {
        #region Unity

        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private Outline outline;
        [SerializeField] private Animator animator;

        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int RunHash = Animator.StringToHash("Run");
        #endregion
        
        public bool CanClick { get; set; } = true;
        public PassengerContent PassengerContent;
        private PassengerGameState _passengerGameState = PassengerGameState.GridState;
        public override void Initialize(CellData cellData, CellContent cellContent)
        {
            PassengerContent = cellContent as PassengerContent;
            base.Initialize(cellData, cellContent);
        }

        public void SetAnimation(PassengerAnimationState animationState)
        {
            switch (animationState)
            {
                case PassengerAnimationState.Idle:
                    animator.Play(IdleHash);
                    break;
                case PassengerAnimationState.Run:
                    animator.Play(RunHash);
                    break;
            }
        }
        
        public void SetColor()
        {
            skinnedMeshRenderer.material.color = PassengerContent.ColorType.GetColor();
        }
        
        public void CheckOutlineVisibility()
        {
            outline.enabled = Pathfinder.Instance.HasAnyPath(CellData.CellPosition);
        }

        private void OnEnable()
        {
            EventManager.Subscribe(GameplayEvents.OnBusArrivedToStop, CheckPassengerAvailability);
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(GameplayEvents.OnBusArrivedToStop, CheckPassengerAvailability);
        }

        private void UpdateCellData(CellData cellData)
        {
            CellData = cellData;
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
                CanClick = false;
                MoveAlongPath(path); 
                EventManager<ItemRemoveData>.Execute(GameplayEvents.OnCellItemRemoved, new ItemRemoveData(CellData));
                EventManager.Execute(GameplayEvents.OnPassengerMove);
            }
            else
            {
                Debug.Log("No path was found or all exits were occupied.");
            }
        }
        
        private void MoveAlongPath(List<CellPosition> path)
        {
            SetAnimation(PassengerAnimationState.Run);
            var followPathSequence = DOTween.Sequence();
            for (int i = 0; i < path.Count; i++)
            {
                followPathSequence.Append(transform.DOMove(path[i].WorldPosition, 0.25f).SetEase(Ease.Linear));
            }
            followPathSequence.OnComplete(DecidePath);
        }

        private void CheckPassengerAvailability()
        {
            if (_passengerGameState != PassengerGameState.BoardingState || !BusController.Instance.CurrentBus.CanGetOn(this))
            {
                return;
            }
            MoveBus();
        }
        
        private void DecidePath()
        {
            var canGetOn = BusController.Instance.CurrentBus.CanGetOn(this);
            if (canGetOn)
            {
                MoveBus();
            }
            else
            {
                MoveBoardingCell();
            }
        }

        private void MoveBus()
        {
            SetAnimation(PassengerAnimationState.Run);
            BusController.Instance.CurrentBus.GetOn(this);
            PassengerController.Instance.DeregisterPassenger(this);
            EventManager<ItemRemoveData>.Execute(GameplayEvents.OnCellItemRemoved, new ItemRemoveData(CellData));
            transform.DOMove(BusController.Instance.CurrentBus.transform.position, 2f).OnComplete(() =>
            {
                transform.SetParent(BusController.Instance.CurrentBus.transform);
                BusController.Instance.CurrentBus.CheckBusState();
                SetState(PassengerGameState.BusState);
                UpdateCellData(null);
                SetAnimation(PassengerAnimationState.Idle);
            });   
        }
        
        private void MoveBoardingCell()
        {
            var targetBoardingCell = GridManager.Instance.GetEligibleBoardingCell();
            targetBoardingCell.FillItem(this);
            UpdateCellData(targetBoardingCell);
            SetState(PassengerGameState.BoardingState);
            transform.DOMove(targetBoardingCell.CellPosition.WorldPosition, 1f).OnComplete(() =>
            {
                SetAnimation(PassengerAnimationState.Idle);
            });
        }

        private void SetState(PassengerGameState passengerGameState)
        {
            _passengerGameState = passengerGameState;
        }
    }
}