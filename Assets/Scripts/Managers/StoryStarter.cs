using Assets.Scripts.Managers;
using System.Collections;
using TMPro;
using UnityEngine;

public class StoryStarter : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private CanvasGroup fullScreenCanvasGroup;

    [Header("Typewriter Settings")]
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private string audioSFXName = "TypewriterChar";

    [Header("Story Pages")]
    [TextArea(5, 10)]
    [SerializeField] private string[] storyPages;

    private int currentPageIndex = 0;
    private bool isPageTyping = false;
    private bool isPageSkipped = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        InputManager.Instance.enabled = false;
        if (storyPages.Length > 0)
        {
            ShowPage(0);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (isPageTyping)
                isPageSkipped = true;
            else
                NextPage();
        }
    }

    private void ShowPage(int pageIndex)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(PlayPageSequence(storyPages[pageIndex]));
    }

    private IEnumerator PlayPageSequence(string pageText)
    {
        isPageTyping = true;
        isPageSkipped = false;

        storyText.text = pageText;

        storyText.ForceMeshUpdate();

        storyText.maxVisibleCharacters = 0;

        int totalVisibleCharacters = storyText.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleCharacters; i++)
        {
            if (isPageSkipped)
            {
                storyText.maxVisibleCharacters = totalVisibleCharacters;
                break;
            }

            storyText.maxVisibleCharacters = i;

            if (AudioManager.Instance != null && i < totalVisibleCharacters)
            {
                AudioManager.Instance.PlaySFX(audioSFXName);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isPageTyping = false;
    }

    private void NextPage()
    {
        currentPageIndex++;

        if (currentPageIndex < storyPages.Length)
            ShowPage(currentPageIndex);
        else
            FinishIntro();
    }

    private void FinishIntro()
    {
        InputManager.Instance.enabled = true;

        if (fullScreenCanvasGroup != null)
        {
            LeanTween.alphaCanvas(fullScreenCanvasGroup, 0f, 1f)
                .setOnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}