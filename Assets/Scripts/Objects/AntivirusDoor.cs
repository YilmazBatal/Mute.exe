using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using TMPro;
using UnityEngine;

public class AntivirusDoor : MonoBehaviour, IInteractable
{
    public bool CanInteract => true;
    public int fragmentRequired = 1;
    private TMP_Text fragmentText;
    private SpriteOutline spriteOutline;

    private void Awake()
    {
        spriteOutline = GetComponent<SpriteOutline>();
        fragmentText = transform.GetChild(0).GetComponent<TMP_Text>();
    }
    private void Start()
    {
        UpdateFragmentText(GameManager.Instance.fragments);
    }

    private void OnEnable()
    {
        EventManager.OnFragmentChanged += UpdateFragmentText;
    }
    private void OnDisable()
    {
        EventManager.OnFragmentChanged -= UpdateFragmentText;
    }
    private void UpdateFragmentText(int currentFragments)
    {
        fragmentText.text = $"{currentFragments}/{fragmentRequired}";
    }

    public void OnInteract()
    {
        if (GameManager.Instance.fragments >= fragmentRequired)
            UnlockDoor(gameObject, 1f, 0f, 0.4f);
        else
            Extensions.FailEffect(UIManager.Instance.volume, this);
    }

    public void OnRangeEnter()
    {
        spriteOutline.outlineSize = 1;
    }

    public void OnRangeExit()
    {
        spriteOutline.outlineSize = 0;
    }

    public void OnRangeStay()
    {
        spriteOutline.outlineSize = 1;
    }
    public void UnlockDoor(GameObject gameObject, float from, float to, float duration)
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null)
            return;
        LeanTween.value(gameObject, from, to, duration)
            .setEaseInOutCubic()
            .setOnUpdate((float value) =>
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, value);
            })
            .setOnComplete(() =>
            {
                if (to <= 0.01f)
                {
                    Destroy(gameObject);
                }
            }); ;
    }
}
