namespace Assets.Scripts.Interfaces
{
    public interface IInteractable
    {
        bool CanInteract { get; }
        void OnRangeEnter();
        void OnRangeStay();
        void OnRangeExit();
        void OnInteract();
    }
}
