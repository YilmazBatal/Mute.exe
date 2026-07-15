using Assets.Scripts;
using Assets.Scripts.Interfaces;
using UnityEngine;

public class PuzzleChip : MonoBehaviour, IInteractable
{
    [SerializeField] public Minigames minigame;

    public bool isCompleted;
    public bool CanInteract => !isCompleted;
    
    [SerializeField] GameObject visualCue;
    [SerializeField] Sprite disabledSprite;

    private SpriteOutline outline;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        outline = GetComponent<SpriteOutline>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (!isCompleted)
        {
            UIManager.Instance.LaunchMinigame(minigame, this);
        }
    }

    public void CompletePuzzle()
    {
        isCompleted = true;
        outline.color = Color.red;
        DisableChip();
        if (visualCue.activeSelf)
        {
            Extensions.ScaleUpDown(visualCue, visualCue.transform.localScale.x, 0f, 0.2f);
        }
    }

    private void DisableChip()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        animator.enabled = false;
        spriteRenderer.sprite = disabledSprite;
    }
}
