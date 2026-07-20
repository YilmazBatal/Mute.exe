using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Audio Mixer & Sources")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource loopingSource;

    [Header("Ses Listeleri")]
    [SerializeField] private Sound[] sfxList;
    [SerializeField] private Sound[] musicList;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadVolume();
    }

    public void PlaySFX(string soundName)
    {
        Sound s = Array.Find(sfxList, item => item.name == soundName);

        if (s.clip != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.LogWarning($"SFX couldn't be found: {soundName}");
        }
    }
    public void PlayLoopingSFX(string soundName)
    {
        if (loopingSource.isPlaying && loopingSource.clip != null && loopingSource.clip.name == soundName)
        {
            return;
        }

        Sound s = Array.Find(sfxList, item => item.name == soundName);

        if (s.clip != null)
        {
            loopingSource.clip = s.clip;
            loopingSource.loop = true;
            loopingSource.Play();
        }
        else
        {
            Debug.LogWarning($"Looping SFX couldn't be found: {soundName}");
        }
    }
    public void StopLoopingSFX()
    {
        loopingSource.Stop();
    }

    public void PlayMusic(string musicName)
    {
        Sound s = Array.Find(musicList, item => item.name == musicName);

        if (s.clip != null)
        {
            musicSource.clip = s.clip;
            musicSource.Play();
            Debug.LogWarning($"Supposed to play: {musicName}");
        }
        else
        {
            Debug.LogWarning($"Music couldn't be found: {musicName}");
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20f);
        PlayerPrefs.SetFloat(MASTER_KEY, sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20f);
        PlayerPrefs.SetFloat(MUSIC_KEY, sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20f);
        PlayerPrefs.SetFloat(SFX_KEY, sliderValue);
    }

    private void LoadVolume()
    {
        float masterVol = PlayerPrefs.GetFloat(MASTER_KEY, 0.75f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        audioMixer.SetFloat("MasterVol", Mathf.Log10(masterVol) * 20f);
        audioMixer.SetFloat("MusicVol", Mathf.Log10(musicVol) * 20f);
        audioMixer.SetFloat("SFXVol", Mathf.Log10(sfxVol) * 20f);
    }
}