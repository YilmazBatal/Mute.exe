using Assets.Scripts;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class SignalCatch : MonoBehaviour
{
    [SerializeField] private CheckInteract bug;

    [Header("Start & End")]
    [SerializeField] private RectTransform wifi;
    [SerializeField] private RectTransform folder;
    [Header("Signal & Properties")]
    [SerializeField] private RectTransform signalRect; // positioning
    [SerializeField] private Image signalImage; // color, opacity, sprite
    bool isGameOver = false;

    [Header("UI Elements")]
    [SerializeField] private Button closeBTN;
    [SerializeField] private Button confirmBTN;
    [SerializeField] private Button sizeBTN;
    [SerializeField] private Button colorBTN;
    [SerializeField] private Button powerBTN;

    [Header("Config")]
    [SerializeField] private Sprite[] signalSize = new Sprite[2]; //  [S], [M], [L]
    [SerializeField] private Color[] signalColors = new Color[2]; // red, orange, yellow
    [SerializeField] private Sprite[] signalPower = new Sprite[1]; // [0], [1]

    [SerializeField] private Sprite[] signalSprites = new Sprite[2]; //  ), )), )))



    [Header("Generated Answers & Player Choices")]
    private Sprite signalSizeCurrent;
    private Color signalColorCurrent;
    private bool signalPowerCurrent;

    private Sprite signalSizeChoice;
    private Color signalColorChoice;
    private bool signalPowerChoice;

    GameManager gm => GameManager.Instance;
    UIManager um => UIManager.Instance;

    #region Button Functions
    public void OnSizeButtonClick()
    {
        // Cycle through the sizes
        int currentIndex = System.Array.IndexOf(signalSize, signalSizeChoice);
        int nextIndex = (currentIndex + 1) % signalSize.Length;
        signalSizeChoice = signalSize[nextIndex];
        // Update the UI to reflect the choice
        sizeBTN.GetComponent<Image>().sprite = signalSizeChoice;
    }
    public void OnColorButtonClick()
    {
        // Cycle through the colors
        int currentIndex = System.Array.IndexOf(signalColors, signalColorChoice);
        int nextIndex = (currentIndex + 1) % signalColors.Length;
        signalColorChoice = signalColors[nextIndex];
        // Update the UI to reflect the choice
        colorBTN.GetComponent<Image>().color = signalColorChoice;
    }
    public void OnPowerButtonClick()
    {
        // Toggle the power state
        signalPowerChoice = !signalPowerChoice;
        // Update the UI to reflect the choice
        powerBTN.GetComponent<Image>().sprite = signalPowerChoice ? signalPower[1] : signalPower[0];
    }
    private void OnConfirmButtonClick()
    {
        if (signalSizeChoice == signalSizeCurrent && signalColorChoice == signalColorCurrent && signalPowerChoice == signalPowerCurrent)
        {
            WinGame();
        }
        else
        {
            Extensions.FailEffect(um.volume, this);
            um.CloseMinigame(minigame: Minigames.SignalCatch, false);
            Debug.Log("Incorrect choice. Try again!");
            Debug.Log("Size : " + signalSizeCurrent);
            Debug.Log("Color : " + signalColorCurrent);
            Debug.Log("Power : " + signalPowerCurrent);
        }
    }
    private void AssignButtonFunctions()
    {
        sizeBTN.onClick.RemoveAllListeners();
        sizeBTN.onClick.AddListener(OnSizeButtonClick);
        colorBTN.onClick.RemoveAllListeners();
        colorBTN.onClick.AddListener(OnColorButtonClick);
        powerBTN.onClick.RemoveAllListeners();
        powerBTN.onClick.AddListener(OnPowerButtonClick);
        closeBTN.onClick.RemoveAllListeners();
        closeBTN.onClick.AddListener(() => um.CloseMinigame(minigame: Minigames.SignalCatch, didPlayerWin: false) );
        confirmBTN.onClick.RemoveAllListeners();
        confirmBTN.onClick.AddListener(OnConfirmButtonClick);
    }
    #endregion

    private void OnEnable()
    {
        isGameOver = false;

        signalSizeChoice = signalSize[0];
        sizeBTN.GetComponent<Image>().sprite = signalSizeChoice;

        signalColorChoice = signalColors[0];
        colorBTN.GetComponent<Image>().color = signalColorChoice;

        signalPowerChoice = false;
        powerBTN.GetComponent<Image>().sprite = signalPower[0];

        RandomizeTheGame();
        AssignButtonFunctions();

        EventManager.OnFragmentChanged += OnFragmentChanged;
    }

    private void OnDisable()
    {
        LeanTween.cancel(signalRect.gameObject);
        signalRect.position = wifi.position;

        EventManager.OnFragmentChanged -= OnFragmentChanged;
    }

    private int currentSignalIndex;

    private void RandomizeTheGame()
    {
        currentSignalIndex = Random.Range(0, signalSize.Length);

        signalSizeCurrent = signalSize[currentSignalIndex];
        signalColorCurrent = signalColors[Random.Range(0, signalColors.Length)];

        signalPowerCurrent = Random.value > 0.5f;

        GenerateNewSignal();
    }

    private void GenerateNewSignal()
    {
        LeanTween.cancel(signalRect.gameObject);

        signalImage.sprite = signalSprites[currentSignalIndex];

        Color targetColor = signalColorCurrent;
        targetColor.a = 1f;
        signalImage.color = targetColor;

        signalRect.position = wifi.position;

        if (!signalPowerCurrent)
        {
            LeanTween.alpha(signalRect, 0f, 0.2f)
                .setFrom(0.5f)                   
                .setLoopPingPong()               
                .setEaseInOutSine();
        }

        signalRect.position = wifi.position;

        LeanTween.move(signalRect.gameObject, folder.position, 3f)
            .setEaseInOutSine()
            .setLoopClamp();
    }
    private void OnFragmentChanged(int fragments)
    {
        um.fragmentText.text = $"Fragments: {fragments}/{gm.maxFragments}";
        Extensions.ZoomInOut(um.fragmentText.transform.parent.gameObject, 1.2f, 0.75f);
    }
    private void WinGame()
    {
        isGameOver = true;

        LeanTween.cancel(signalRect.gameObject);
        Color c = signalImage.color;
        c.a = 0f;
        signalImage.color = c;

        GameManager.Instance.fragments += 1;
        EventManager.TriggerFragmentChanged(GameManager.Instance.fragments);

        Debug.Log("Minigame Complete!");
        um.CloseMinigame(minigame: Minigames.SignalCatch, didPlayerWin: true);
    }
}