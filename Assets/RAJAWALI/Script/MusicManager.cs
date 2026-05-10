using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;


public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Settings")]
    public AudioSource musicSource;
    public AudioMixer audioMixer;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip deepSpaceMusic;
    public AudioClip asteroidFieldMusic;
    public AudioClip capitalShipBattleMusic;
    public AudioClip loadoutArcadeMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip level4Music;
    public AudioClip level5Music;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusic(mainMenuMusic);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "LoadoutInstantAction":
                PlayMusic(mainMenuMusic);
                break;
            case "LoadoutTutorial":
                PlayMusic(mainMenuMusic);
                break;
            case "Deepspace":
                PlayMusic(deepSpaceMusic);
                break;
            case "AsteroidField":
                PlayMusic(asteroidFieldMusic);
                break;
            case "CapitalShipBattle":
                PlayMusic(capitalShipBattleMusic);
                break;
            case "LoadoutArcade":
                PlayMusic(loadoutArcadeMusic);
                break;
            case "Level1":
                PlayMusic(level1Music);
                break;
            case "Level2":
                PlayMusic(level2Music);
                break;
            case "Level3":
                PlayMusic(level3Music);
                break;
            case "Level4":
                PlayMusic(level4Music);
                break;
            case "Level5":
                PlayMusic(level5Music);
                break;
        }

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
    }

    private Coroutine fadeRoutine;

    public void PlayMusic(AudioClip newMusic)
    {
        if (newMusic == null) return;
        if (musicSource.clip == newMusic) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeMusic(newMusic));
    }

    private IEnumerator FadeMusic(AudioClip newMusic)
    {
        float fadeOutTime = 1f;
        float fadeInTime = 1f;
        float targetVolume = 1f;

        // Fade out current song
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(targetVolume, 0, t / fadeOutTime);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop();

        // Change song
        musicSource.clip = newMusic;
        musicSource.loop = true;

        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("Music");
        if (groups.Length > 0)
            musicSource.outputAudioMixerGroup = groups[0];

        musicSource.Play();

        // Fade in new song
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, targetVolume, t / fadeInTime);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    
}
