using System;
using BusJamDemo.Service;
using UnityEngine;


namespace BusJamDemo.Utility
{

    public class CameraAdjuster : MonoBehaviour
    {
        [SerializeField] private Vector3 basePosition = new Vector3(0f, 10f, -7f);
        [SerializeField] private Vector3 baseRotation = new Vector3(45f, 0f, 0f);
        [SerializeField] private float baseFOV = 60f;

        [SerializeField] private float baseHeight = 10f;
        [SerializeField] private float baseDistance = 7f;
        
        [SerializeField] private float heightFactor = 0.5f;
        [SerializeField] private float distanceFactor = 0.5f;
        
        private Camera _targetCamera;
        private ILevelService _levelService;
        public void Initialize(ILevelService levelService)
        {
            _levelService = levelService;
            _targetCamera = Camera.main;
            EventManager.Subscribe(GameplayEvents.LevelLoaded, AdjustCamera);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameplayEvents.LevelLoaded, AdjustCamera);
        }
        
        private void AdjustCamera()
        {
            if (_targetCamera == null || _levelService == null)
                return;
        
            int rows = _levelService.CurrentLevelData.Rows;
            int columns = _levelService.CurrentLevelData.Columns;
            float cellSize = _levelService.CurrentLevelData.CellSize;
        
            float gridWidth = (columns - 1) * cellSize;
            float gridHeight = (rows - 1) * cellSize;
        
            float totalSize = gridWidth + gridHeight;
        
            float newY = baseHeight + totalSize * heightFactor;
            float newZ = -baseDistance - totalSize * distanceFactor;
        
            Vector3 newPos = new Vector3(0f, newY, newZ);
        
            _targetCamera.transform.position = newPos;
            _targetCamera.transform.rotation = Quaternion.Euler(baseRotation);
            _targetCamera.fieldOfView = baseFOV;
        }
    }
}