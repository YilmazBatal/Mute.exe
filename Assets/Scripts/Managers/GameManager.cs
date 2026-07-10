using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameObject bug;
        [SerializeField] public int fragments = 0;
        [SerializeField] public int maxFragments;

        [SerializeField] public PuzzleChip puzzleChip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Initialize();
        }
        private void Initialize()
        {
            bug = GameObject.FindGameObjectWithTag("Player");
            fragments = 0;
        }
    }
}
