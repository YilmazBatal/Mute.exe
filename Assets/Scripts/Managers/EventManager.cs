using System;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public static class EventManager
    {
        public static class CombatEvents
        {
            public static event Action<Material> OnEntityDamaged;
            public static void EntityDamaged(Material flashMaterial) => OnEntityDamaged?.Invoke(flashMaterial);
        }
        public static class GameEvents
        {
            public static event Action<int> OnFragmentChanged;
            public static event Action<Transform> OnPlayerSpawned;

            public static void TriggerFragmentChanged(int currentFragments) => OnFragmentChanged?.Invoke(currentFragments);
            public static void PlayerSpawned(Transform playerTransform) => OnPlayerSpawned?.Invoke(playerTransform);
        }
    }
}
