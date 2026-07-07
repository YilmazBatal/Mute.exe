using Assets.Scripts;
using Assets.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VariableMatchManager : MonoBehaviour
{
    [SerializeField] private CheckInteract bug;

    [Header("Borders & Movement")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private RectTransform mainBar; // Ensure this has a Horizontal Layout Group!
    bool isGameOver = false;

    [Header("UI Elements")]
    [SerializeField] private Button closeBTN;
    [SerializeField] private TMP_Text matchText;    // e.g., Shows "0/4"
    [SerializeField] private TMP_Text generatedVar; // e.g., Shows "Value 1 : True" or "Value 1 : 42"
    private int correctMatches = 0;
    private int totalMatches = 1;

    [Header("Config")]
    [SerializeField] private float minWidth = 50f;
    [SerializeField] private float maxWidth = 150f;

    [Header("Variables (UI Blocks)")]
    [SerializeField] private RectTransform boolVar;
    [SerializeField] private RectTransform stringVar;
    [SerializeField] private RectTransform integerVar;
    [SerializeField] private RectTransform floatVar;

    [Header("Colors")]
    [SerializeField] private Color boolColor;
    [SerializeField] private Color stringColor;
    [SerializeField] private Color integerColor;
    [SerializeField] private Color floatColor;

    private List<RectTransform> variables;
    private List<Color> variableColors;
    private List<string> variableTypes = new List<string> { "Bool", "String", "Integer", "Float" };

    // Track the target type the player needs to hit right now
    private string currentTargetType;

    GameManager gm => GameManager.Instance;
    UIManager um => UIManager.Instance;

    private void OnEnable()
    {
        variables = new List<RectTransform> { boolVar, stringVar, integerVar, floatVar };
        variableColors = new List<Color> { boolColor, stringColor, integerColor, floatColor };

        correctMatches = 0;
        UpdateMatchText();

        RandomizeTheGame();
        HandleArrow();

        closeBTN.onClick.RemoveAllListeners();
        closeBTN.onClick.AddListener(() =>
        {
            um.CloseMinigame(minigame: Minigames.VariableMatch, didPlayerWin: false);
        });

        EventManager.OnFragmentChanged += OnFragmentChanged;
    }
    private void OnDisable()
    {
        EventManager.OnFragmentChanged -= OnFragmentChanged;

    }

    private void RandomizeTheGame()
    {
        // 1. Shuffle the hierarchy layout using Sibling Index so the blocks are in a random order
        List<int> indexes = new List<int> { 0, 1, 2, 3 };
        for (int i = 0; i < variables.Count; i++)
        {
            int randomIndex = indexes[Random.Range(0, indexes.Count)];
            variables[i].SetSiblingIndex(randomIndex);
            indexes.Remove(randomIndex);

            // 2. Assign sizes and colors
            variables[i].sizeDelta = new Vector2(Random.Range(minWidth, maxWidth), variables[i].sizeDelta.y);
            variables[i].GetComponent<Image>().color = variableColors[i];
        }

        // Force the layout group to update coordinates immediately
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainBar);

        // 3. Pick a random question type
        GenerateNewQuestion();
    }

    private void GenerateNewQuestion()
    {
        // Pick a random data type out of the 4 available
        currentTargetType = variableTypes[Random.Range(0, variableTypes.Count)];

        // Generate a fun dummy value based on the type chosen
        string randomValueDisplay = "";
        switch (currentTargetType)
        {
            case "Bool":
                randomValueDisplay = Random.value > 0.5f ? "True" : "False";
                break;
            case "String":
                string[] words = { "\"Hello\"", "\"Data\"", "\"Unity\"", "\"Code\"" };
                randomValueDisplay = words[Random.Range(0, words.Length)];
                break;
            case "Integer":
                randomValueDisplay = Random.Range(1, 99).ToString();
                break;
            case "Float":
                randomValueDisplay = Random.Range(1.0f, 99.0f).ToString("F2") + "f";
                break;
        }

        generatedVar.text = $"Value {correctMatches + 1} : <color=#FFCC00>{randomValueDisplay}</color>";
    }

    private void HandleArrow()
    {
        if (!isGameOver)
        {
            LeanTween.value(arrow.gameObject, -465f, 465f, 2f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnUpdate((float value) =>
            {
                arrow.anchoredPosition = new Vector2(value, arrow.anchoredPosition.y);
            });
        }
    }

    public void AssignButton()
    {
        if (isGameOver) return;

        // Find the block that corresponds to our target type
        RectTransform targetBlock = null;
        if (currentTargetType == "Bool") targetBlock = boolVar;
        else if (currentTargetType == "String") targetBlock = stringVar;
        else if (currentTargetType == "Integer") targetBlock = integerVar;
        else if (currentTargetType == "Float") targetBlock = floatVar;

        if (targetBlock != null)
        {
            // Calculate the block's exact boundaries relative to the mainBar container
            float blockLeftEdge = targetBlock.anchoredPosition.x - (targetBlock.sizeDelta.x / 2f);
            float blockRightEdge = targetBlock.anchoredPosition.x + (targetBlock.sizeDelta.x / 2f);

            // Check if the arrow is currently inside those bounds
            float arrowX = arrow.anchoredPosition.x;

            if (arrowX >= blockLeftEdge && arrowX <= blockRightEdge)
            {
                Debug.Log("Correct Match!");
                correctMatches++;
                UpdateMatchText();

                if (correctMatches >= totalMatches)
                    WinGame(gm.fragments);
                else
                    GenerateNewQuestion();
            }
            else
            {
                Debug.Log("Missed! Try again.");
                Extensions.Shake(this.gameObject.GetComponent<RectTransform>(), this, 0.5f, 10f);
                Extensions.FailEffect(um.volume, um);
                um.CloseMinigame(minigame: Minigames.VariableMatch, didPlayerWin: false);
            }
        }
    }

    private void UpdateMatchText()
    {
        matchText.text = $"{correctMatches}/{totalMatches}";
    }
    private void OnFragmentChanged(int fragments)
    {
        um.fragmentText.text = $"Fragments: {fragments}/{gm.maxFragments}";
        Extensions.ZoomInOut(um.fragmentText.transform.parent.gameObject, 1.2f, 0.75f);
    }
    private void WinGame(int fragments)
    {
        isGameOver = true;
        LeanTween.cancel(arrow.gameObject);
        generatedVar.text = "SUCCESSFULY HACKED!";
        //bug.lastActivePuzzle.GetComponent<GameSelecter>().isCompleted = true;
        bug.lastActivePuzzle.GetComponent<SpriteOutline>().outlineSize = 0;

        fragments += 1;
        EventManager.TriggerFragmentChanged(fragments);

        Debug.Log("Minigame Complete!");
        um.CloseMinigame(minigame: Minigames.VariableMatch, didPlayerWin: true);
    }
}