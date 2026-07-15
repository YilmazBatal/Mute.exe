using Assets.Scripts.Managers;
using UnityEngine;
public class SceneInitializer : MonoBehaviour
{

    [Header("Initializer")]
    [SerializeField] private int requiredFragmentsForThisLevel = 3; 
    [SerializeField] private string levelName = "Maze 1";
    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Initialize(requiredFragmentsForThisLevel, levelName);
        Destroy(gameObject);
    }

}

