namespace Assets.Scripts.Interfaces
{
    public interface IInteractable
    {
        void OnRangeEnter();
        void OnRangeStay();
        void OnRangeExit();
        void OnInteract();
    }
}
