using Assets.Scripts.Managers;
using UnityEngine;

/// <summary>
/// transfer the scanning work to the player later
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    InputSystem_Actions controls => InputManager.Instance.controls;
    [SerializeField] GameObject visualCue;
    [SerializeField] TextAsset inkJSON;
    bool inRange;
    private void Awake()
    {

        inRange = false;
        visualCue.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (inRange)
        {
            visualCue.gameObject.SetActive(true);
            if (controls.Player.Interact.triggered)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            visualCue.gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("dialogue"))
        {
            inRange = true;
            Extensions.ScaleUpDown(visualCue.gameObject, 0, 4f, 0.3f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("dialogue"))
        {
            inRange = false;
            Extensions.ScaleUpDown(visualCue.gameObject, 4f, 0, 0.3f);
        }
    }
}
