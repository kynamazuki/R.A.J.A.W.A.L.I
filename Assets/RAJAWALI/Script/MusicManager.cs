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
            case "DeepSpace_Supreme":
                PlayMusic(deepSpaceMusic);
                break;
            case "AsteroidField_Supreme":
                PlayMusic(asteroidFieldMusic);
                break;
            case "CapitalShipBattle_Supreme":
                PlayMusic(capitalShipBattleMusic);
                break;
        }

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        audioMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
    }

    public void PlayMusic(AudioClip newMusic)
    {
        if (musicSource.clip == newMusic) return;

        musicSource.clip = newMusic;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        musicSource.Play();
        StartCoroutine(FadeMusic(newMusic));
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    private IEnumerator FadeMusic(AudioClip newMusic)
    {
        float fadeOutTime = 1f; // seconds
        float fadeInTime = 1f;
        float startVolume = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeOutTime);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newMusic;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeInTime);
            yield return null;
        }

        musicSource.volume = startVolume;
    }
}
