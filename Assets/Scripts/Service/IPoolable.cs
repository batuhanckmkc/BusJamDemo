namespace BusJamDemo.Service
{
    public interface IPoolable
    {
        void OnReleaseToPool();
        void OnGetFromPool();
    }
}