using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip loseMusic;
    [SerializeField] private AudioClip winMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSFX;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.7f;

    private readonly HashSet<Button> registeredButtons = new HashSet<Button>();

    private void Awake()
    {
        // Prevent duplicate AudioManagers
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForCurrentScene();
        RegisterAllButtons();
    }

    private void Update()
    {
        // Automatically detect buttons created later
        RegisterAllButtons();
    }

    private void SetupAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObject = new GameObject("Music Source");
            musicObject.transform.SetParent(transform);

            musicSource = musicObject.AddComponent<AudioSource>();
        }

        if (sfxSource == null)
        {
            GameObject sfxObject = new GameObject("SFX Source");
            sfxObject.transform.SetParent(transform);

            sfxSource = sfxObject.AddComponent<AudioSource>();
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.spatialBlend = 0f;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
        sfxSource.spatialBlend = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();

        // Automatically add SFX to buttons in the new scene
        RegisterAllButtons();
    }

    private void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        AudioClip newMusic = GetMusicForScene(sceneName);

        if (newMusic == null)
        {
            musicSource.Stop();
            return;
        }

        // Don't restart if the same music is already playing
        if (musicSource.clip == newMusic && musicSource.isPlaying)
            return;

        musicSource.Stop();

        musicSource.clip = newMusic;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    private AudioClip GetMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            // MENU MUSIC
            case "MainMenu":
            case "HowToPlay":
            case "SettingScene":
            case "About us":
            case "LevelManager":
                return menuMusic;

            // GAME MUSIC
            case "GameScene":
                return gameMusic;

            // LOSE MUSIC
            case "LoseScene":
                return loseMusic;

            // WIN MUSIC
            case "WinScene":
                return winMusic;

            default:
                Debug.LogWarning("No music assigned for scene: " + sceneName);
                return null;
        }
    }

    // Automatically gives every button a click sound
    private void RegisterAllButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (button == null)
                continue;

            if (registeredButtons.Contains(button))
                continue;

            button.onClick.AddListener(PlayButtonClick);

            registeredButtons.Add(button);
        }
    }

    private void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}