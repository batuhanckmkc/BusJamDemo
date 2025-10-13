using UnityEngine;

namespace BusJamDemo.UI
{
    public abstract class ScreenUI : MonoBehaviour
    {
        [SerializeField] protected GameObject Root;

        public void Show()
        {
            Root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            Root.gameObject.SetActive(false);
        }
    }
}