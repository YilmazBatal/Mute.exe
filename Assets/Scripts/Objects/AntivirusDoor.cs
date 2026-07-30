using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class AntivirusDoor : MonoBehaviour, IInteractable
{
    public bool CanInteract => true;
    public int fragmentRequired = 1;
    [SerializeField] private TMP_Text fragmentText;
    private SpriteOutline spriteOutline;
    private ParticleSystem smoke;

    private void Awake()
    {
        spriteOutline = GetComponent<SpriteOutline>();
    }
    private void Start()
    {
        UpdateFragmentText(GameManager.Instance.fragments);
    }

    private void OnEnable()
    {
        EventManager.GameEvents.OnFragmentChanged += UpdateFragmentText;
    }
    private void OnDisable()
    {
        EventManager.GameEvents.OnFragmentChanged -= UpdateFragmentText;
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
        {
            AudioManager.Instance.PlaySFX("UIError");
            Extensions.FailEffect(UIManager.Instance.volume, this);
            GameManager.Instance.GetComponent<CinemachineImpulseSource>().DefaultVelocity = new Vector3(0.2f, 0.2f, 0);
            GameManager.Instance.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        }
    }
    public void OnRangeEnter()
    {
        if (spriteOutline != null) spriteOutline.outlineSize = 1;
    }

    public void OnRangeExit()
    {
        if (spriteOutline != null) spriteOutline.outlineSize = 0;
    }

    public void OnRangeStay()
    {
        spriteOutline.outlineSize = 1;
    }
    public void UnlockDoor(GameObject gameObject, float from, float to, float duration)
    {
        GameManager.Instance.GetComponent<CinemachineImpulseSource>().DefaultVelocity = new Vector3(0.1f, 0.1f, 0);
        GameManager.Instance.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        AudioManager.Instance.PlaySFX("UISuccess");

        if (transform.childCount > 0)
            smoke = transform.GetChild(0).GetComponent<ParticleSystem>();

        smoke.Play();

        if (transform.childCount > 0)
            transform.DetachChildren();

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
