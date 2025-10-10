using UnityEngine;

namespace BusJamDemo.Bootstrap
{
    public class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute() {
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
        }
    }
}