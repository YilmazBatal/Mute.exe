using Assets.Scripts.Interfaces;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject visualCue;
    [SerializeField] private TextAsset inkJSON;
    public bool CanInteract => true;

    private void Awake()
    {
        visualCue.transform.localScale = Vector3.zero;
        visualCue.SetActive(false);
    }

    public void OnRangeEnter()
    {
        visualCue.SetActive(true);
        Extensions.ScaleUpDown(visualCue, 0f, 4f, 0.3f);
    }

    public void OnRangeStay()
    {

    }

    public void OnRangeExit()
    {
        Extensions.ScaleUpDown(visualCue, visualCue.transform.localScale.x, 0f, 0.3f);
    }

    public void OnInteract()
    {
        DialogueManager.GetInstance().EnterDialogueMode(inkJSON);

        Extensions.ScaleUpDown(visualCue, visualCue.transform.localScale.x, 0f, 0.2f);
    }
}