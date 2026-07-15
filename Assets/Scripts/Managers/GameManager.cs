using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public GameObject bug;
        [SerializeField] public int fragments = 0;
        [HideInInspector] public int maxFragments;
        [HideInInspector] public string levelName;

        [SerializeField] public PuzzleChip puzzleChip;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            bug = GameObject.FindGameObjectWithTag("Player");

        }
        public void Initialize(int maxFragments, string levelName)
        {
            this.maxFragments = maxFragments;
            this.levelName = levelName;
        }
    }
}
