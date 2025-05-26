using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;  

public class AudioManager : MonoBehaviour
{
    [Header("---AudioSource---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource DialogSource;

    [Header("---AudioClip_Music---")]
    public AudioClip mainMenu;

    [Header("---AudioClip_Sound---")]
    public AudioClip ButtonClick;
    public AudioClip ButtonHover;


    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        

    }


    private void Start()
    {
        ChangeMusic();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusic();
    }

    private void ChangeMusic()
    {
        if (SceneManager.GetActiveScene().name == "Main menu")
        {
            musicSource.clip = mainMenu;
        }
        

        float savedVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 0.75f;

        AudioMixer audioMixer = FindObjectOfType<VolumeSetting>()?.myMixer;

        if (audioMixer != null)
        {
            float mixerVolume;
            if (audioMixer.GetFloat("Music", out mixerVolume))
            {
                musicSource.volume = Mathf.Pow(10, mixerVolume / 20);
            }
            else
            {
                musicSource.volume = savedVolume;
            }
        }
        else
        {
            musicSource.volume = savedVolume;
        }

        if (savedVolume > 0)
        {
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

   


    public void PlaySFX(AudioClip clip)
    {
        SFXSource.clip = clip;
        SFXSource.Play();
    }


    public void StopSFX(AudioClip clip)
    {
        if (SFXSource.isPlaying && SFXSource.clip == clip)
        {
            SFXSource.Stop();
        }
    }


    public void UpdateMusicVolume(float volume)
    {
        musicSource.volume = volume;

        if (volume == 0)
        {
            musicSource.Stop();
        }
        else if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        AudioMixer myMixer = FindObjectOfType<VolumeSetting>().myMixer; 
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20); 
    }


    public void ChangeMusicWithFade(AudioClip newMusic, float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutAndIn(newMusic, fadeDuration));
    }

    private IEnumerator FadeOutAndIn(AudioClip newClip, float fadeDuration)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Stop(); 


        musicSource.clip = newClip;
        musicSource.Play();


        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVolume;
    }

    public void StopMusicWithFade(float fadeDuration = 1f)
    {
        StartCoroutine(FadeOutStopMusic(fadeDuration));
    }

    private IEnumerator FadeOutStopMusic(float fadeDuration)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
        musicSource.Pause();

    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
}


