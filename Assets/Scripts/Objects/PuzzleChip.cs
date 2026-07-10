using Assets.Scripts;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class PuzzleChip : MonoBehaviour, IInteractable
{
    [SerializeField] public Minigames minigame;

    public bool isCompleted;
    public bool CanInteract => !isCompleted;
    
    [SerializeField] GameObject visualCue;

    private SpriteOutline outline;
    
    void Awake()
    {
        outline = GetComponent<SpriteOutline>();
        visualCue.SetActive(false);
    }
    public void OnRangeEnter()
    {
        if (outline != null) outline.outlineSize = 1;
        Extensions.ScaleUpDown(visualCue, 0f, 4f, 0.2f);
    }
    public void OnRangeStay()
    {
        if (outline != null) outline.outlineSize = 1;
    }
    public void OnRangeExit()
    {
        if (outline != null) outline.outlineSize = 0;
        Extensions.ScaleUpDown(visualCue, 4f, 0f, 0.2f);

    }

    public void OnInteract()
    {
        Debug.Log($"Interacted with {gameObject.name}");
        if (!isCompleted)
        {
            UIManager.Instance.LaunchMinigame(minigame, this);
        }
    }

    public void CompletePuzzle()
    {
        isCompleted = true;
        Debug.Log($"{gameObject.name} puzzle is completed!");
        outline.color = Color.red;

        if (visualCue.activeSelf)
        {
            Extensions.ScaleUpDown(visualCue, visualCue.transform.localScale.x, 0f, 0.2f);
        }
    }

}
