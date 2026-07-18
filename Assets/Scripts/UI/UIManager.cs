using Assets.Scripts;
using Assets.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private InputManager im => InputManager.Instance;

    [Header("UI Elements")]
    [SerializeField] public Volume volume;
    [SerializeField] public TMP_Text fragmentText;
    [SerializeField] private TMP_Text interactText;
    
    [Header("Minigames")]
    [SerializeField] private GameObject logicGates;
    [SerializeField] private GameObject variableConnect;
    [SerializeField] private GameObject signalCatch;
    [SerializeField] private GameObject game4;
    [SerializeField] private GameObject game5;

    private PuzzleChip activeChip;
    // Eğer activeChip boş değilse, demek ki ekranda bir minigame oynanıyor!

    [Header("Pause & NextStage UI Elements")]
    [SerializeField] private GameObject pausePanel;
    private bool pauseActive = false;

    [SerializeField] private GameObject nextStageUI;
    private bool nextStageUIActive = false;
    public bool isMinigameActive => activeChip != null;
    private Dictionary<Minigames, GameObject> minigameDict = new Dictionary<Minigames, GameObject>();

    private void Awake()
    {
        if (Instance !=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject)
        minigameDict.Clear();
        minigameDict = new Dictionary<Minigames, GameObject>{
            { Minigames.LogicGates, logicGates },
            { Minigames.VariableMatch, variableConnect },
            { Minigames.SignalCatch, signalCatch },
            { Minigames.Game4, game4 },
            { Minigames.Game5, game5 }
        };


    }
    private void Start()
    {
        if (fragmentText != null)
            fragmentText.text = $"{GameManager.Instance.fragments}/{GameManager.Instance.maxFragments}";

        nextStageUI = GameObject.FindWithTag("NextStage");

    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (im.controls.Player.Cancel.triggered)
                ProccessPausing();
        }
        if (isMinigameActive)
        {
            GameManager.Instance.bug.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            InputManager.Instance.EnableUIControls();
        }
        else
        {
            InputManager.Instance.EnablePlayerControls();
        }
    }
    #region MenuConfig
    public void ProccessNextStageUI()

    {
        nextStageUIActive = !nextStageUIActive;
        if (nextStageUIActive)
        {
            Extensions.OpacityFade(nextStageUI, 0f, 1f, 0.3f);
            nextStageUI.SetActive(true);
        }
        else
            Extensions.OpacityFade(nextStageUI, 1f, 0f, 0.3f);
    }
    private void ProccessPausing()
    {
        pauseActive = !pauseActive;
        if (pauseActive)
        {
            Extensions.OpacityFade(pausePanel, 0f, 1f, 0.3f);
            pausePanel.SetActive(true);
        }
        else
            Extensions.OpacityFade(pausePanel, 1f, 0f, 0.3f);
    }
    public void ContinueBTN()
    {
        ProccessPausing();
    }
    public void MenuBTN()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

    public void ShowInteract(bool value)
    {
        interactText.gameObject.SetActive(value);
    }
    public void LaunchMinigame(Minigames minigame, PuzzleChip triggerChip)
    {
        activeChip = triggerChip;

        minigameDict[minigame].SetActive(true);
        minigameDict[minigame].GetComponent<Transform>().localScale = Vector3.zero;
        LeanTween.value(gameObject, (float value) =>
        {
            minigameDict[minigame].GetComponent<Transform>().localScale = new Vector3(value, value, value);
        }, 0f, 1f, 0.3f).setEase(LeanTweenType.easeInOutCubic);

    }
    public void CloseMinigame(Minigames minigame, bool didPlayerWin)
    {   
        LeanTween.value(gameObject, (float value) =>
        {
            minigameDict[minigame].GetComponent<Transform>().localScale = new Vector3(value, value, value);
        }, 1f, 0f, 0.3f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() =>
        {
            minigameDict[minigame].SetActive(false);

            if (didPlayerWin && activeChip != null)
            {
                activeChip.CompletePuzzle();
            }
            activeChip = null;
        });
    }
    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        LeanTween.value(panel, (float value) =>
        {
            panel.transform.localScale = new Vector3(value, value, value);
        }, 0f, 1f, 0.3f).setEase(LeanTweenType.easeInOutCubic);
    }
    public void ClosePanel(GameObject panel)
    {
        LeanTween.value(panel, (float value) =>
        {
            panel.transform.localScale = new Vector3(value, value, value);
        }, 1f, 0f, 0.3f).setEase(LeanTweenType.easeInOutCubic)
        .setOnComplete(() =>
        {
            panel.SetActive(false);
        });
    }
}
