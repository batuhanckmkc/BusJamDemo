namespace BusJamDemo.Core.Input
{
    public interface IClickable
    {
        bool CanClick { get; set; }
        void HandleClick();
    }
}