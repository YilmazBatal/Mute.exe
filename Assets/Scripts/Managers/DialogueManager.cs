using Assets.Scripts.Managers;
using Ink.Runtime;
using System;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    InputSystem_Actions controls => InputManager.Instance.controls;

    [Header("Dialogue UI")] 
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text dialogueTalker;
    private Story currentStory;
    private bool dialogueIsPlaying = false;
    private void Awake()
    {
        Instance = this;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public static DialogueManager GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }
    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }
        if (controls.Player.SkipDialogue.triggered)
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = string.Empty;
    }
}
