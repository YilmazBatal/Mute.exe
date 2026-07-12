using Assets.Scripts;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class HackingChip : MonoBehaviour, IInteractable
{
    public bool isCompleted;
    GameManager gm => GameManager.Instance;
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
        visualCue.transform.localScale = Vector3.one * 4;
    }
    public void OnRangeExit()
    {
        if (outline != null) outline.outlineSize = 0;
        Extensions.ScaleUpDown(visualCue, 4f, 0f, 0.2f);

    }

    public void OnInteract()
    {
        Debug.Log($"Interacted with {gameObject.name}");

        if (gm.fragments == gm.maxFragments)
        {
            Debug.Log($"Maze 1 is completed. you can code a fun boss fight or just tp to the next area since u are in a hurry. for now ill make a next level menu");
        }
        else
        {
            Debug.Log($"Find the rest of the code fragments. you dont have enough data to hack this network");

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
