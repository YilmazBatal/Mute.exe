using System;

namespace Assets.Scripts.Managers
{
    public static class EventManager
    {
        public static event Action<int> OnFragmentChanged;

        public static void TriggerFragmentChanged(int currentFragments) => OnFragmentChanged?.Invoke(currentFragments);
    }
}
