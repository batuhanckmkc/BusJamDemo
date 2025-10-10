using UnityEngine;

namespace BusJamDemo.Core.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        private readonly float _raycastMaxDistance = 100f;
        private void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                HandleClickInput(UnityEngine.Input.mousePosition);
            }
        }

        private void HandleClickInput(Vector3 screenPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _raycastMaxDistance, targetLayer))
            {
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.HandleClick();
                }
            }
        }
    }
}