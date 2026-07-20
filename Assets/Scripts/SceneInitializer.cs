using Assets.Scripts.Managers;
using UnityEngine;
public class SceneInitializer : MonoBehaviour
{

    [Header("Initializer")]
    [SerializeField] private int requiredFragmentsForThisLevel; 
    [SerializeField] private string levelName = "Maze 1";
    private void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Initialize(requiredFragmentsForThisLevel, levelName);
            
        // Prevent destroying the GameManager if attached to the same GameObject
        if (GetComponent<GameManager>() != null)
            Destroy(this);
        else
            Destroy(gameObject);
    }

}

