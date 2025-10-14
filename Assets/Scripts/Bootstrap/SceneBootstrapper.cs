using UnityEngine;

namespace BusJamDemo.Bootstrap
{
        public class SceneBootstrapper : MonoBehaviour
        {
            private void Awake()
            {
                if (GameObject.Find("Systems(Clone)") == null)
                {
                    Bootstrapper.Execute();
                }
                Destroy(gameObject);
            }
        }
}