using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance { get; private set; }

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private RectTransform loadingIcon;

    private void Start()
    {
        MusicPlayer();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        
    }
    public void NextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
    public void OpenPanel(GameObject panel)
    {
        UIManager.Instance.OpenPanel(panel);
    }
    public void ClosePanel(GameObject panel)
    {
        UIManager.Instance.ClosePanel(panel);
    }
    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
    public void QuitToDesktop()
    {
        Application.Quit();
    }
    
    private void MusicPlayer()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            AudioManager.Instance.PlayMusic("MenuMusic");
        } else
        {
            AudioManager.Instance.PlayMusic("GameMusic");
        }
    }
    public void PlayEasterEgg()
    {
        AudioManager.Instance.PlaySFX("fart");
    }
}