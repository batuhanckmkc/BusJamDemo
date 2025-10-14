using System;
using System.Collections.Generic;
using BusJamDemo.BusSystem;
using BusJamDemo.Core;
using BusJamDemo.Core.Input;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
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

        private IPathfindingService _pathfinder;
        private IGridService _gridService;
        private IBusService _busService;
        private IGameService _gameService;
        
        public void InitializeServices(IPathfindingService pathfinder, IGridService gridService, IBusService busService, IGameService gameService)
        {
            _pathfinder = pathfinder;
            _gridService = gridService;
            _busService = busService;
            _gameService = gameService;
            
            _gameService.OnGameStateChanged += OnGameStateChanged;
        }
        
        public override void Initialize(CellData cellData, CellContent cellContent)
        {
            PassengerContent = cellContent as PassengerContent;
            EventManager.Subscribe(GameplayEvents.OnBusArrivedToStop, CheckPassengerAvailability);
            base.Initialize(cellData, cellContent);
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.LevelFail)
            {
                EventManager.Unsubscribe(GameplayEvents.OnBusArrivedToStop, CheckPassengerAvailability);
            }
        }
        
        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameplayEvents.OnBusArrivedToStop, CheckPassengerAvailability);
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
            outline.enabled = _pathfinder.HasAnyPath(CellData.CellPosition);
        }
        
        private void UpdateCellData(CellData cellData)
        {
            CellData = cellData;
        }
        
        public void HandleClick()
        {
            if (_passengerGameState != PassengerGameState.GridState) return;
            
            var path = _pathfinder.FindPath(CellData.CellPosition);
            if (path == null)
            {
                SetAnimation(PassengerAnimationState.Idle);
                return;
            }
            
            CanClick = false;
            SetAnimation(PassengerAnimationState.Run);
            EventManager<ItemRemoveData>.Execute(GameplayEvents.OnCellItemRemoved, new ItemRemoveData(CellData));
            EventManager<Passenger>.Execute(GameplayEvents.OnPassengerMove, this);
            MoveAlongPath(path);
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

        public void Stop()
        {
            CanClick = false;
            SetAnimation(PassengerAnimationState.Idle);
        }
        
        private void CheckPassengerAvailability()
        {
            if (_passengerGameState != PassengerGameState.BoardingState || !_busService.CurrentBus.CanGetOn(this))
            {
                return;
            }
            MoveToBus();
        }
        
        private void DecidePath()
        {
            var canGetOn = _busService.CurrentBus.CanGetOn(this);
            if (canGetOn)
            {
                MoveToBus();
            }
            else
            {
                MoveBoardingCell();
            }
        }

        private void MoveToBus()
        {
            SetAnimation(PassengerAnimationState.Run);
            _busService.CurrentBus.GetOn(this);
            EventManager<ItemRemoveData>.Execute(GameplayEvents.OnCellItemRemoved, new ItemRemoveData(CellData));
            transform.DOMove(_busService.StopPosition, 2f).OnComplete(() =>
            {
                transform.SetParent(_busService.CurrentBus.transform);
                _busService.CurrentBus.CheckBusState();
                SetState(PassengerGameState.BusState);
                UpdateCellData(null);
                SetAnimation(PassengerAnimationState.Idle);
            });   
        }
        
        private void MoveBoardingCell()
        {
            var targetBoardingCell = _gridService.GetEligibleBoardingCell();
            if (targetBoardingCell == null)
            {
                return;
            }
            targetBoardingCell.FillItem(this);
            UpdateCellData(targetBoardingCell);
            SetState(PassengerGameState.BoardingState);
            transform.DOMove(targetBoardingCell.CellPosition.WorldPosition, 1f).OnComplete(() =>
            {
                SetAnimation(PassengerAnimationState.Idle);
                if (_gridService.AllBoardingCellsIsBusy())
                {
                    _gameService.UpdateGameState(GameState.LevelFail);
                }
            });
        }

        private void SetState(PassengerGameState passengerGameState)
        {
            _passengerGameState = passengerGameState;
        }
    }
}