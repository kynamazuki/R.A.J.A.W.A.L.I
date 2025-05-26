using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider DialogSlider;

    private void Start()
    {
        LoadVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        FindObjectOfType<AudioManager>()?.UpdateMusicVolume(volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetDialogVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("Dialog", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("DialogVolume", volume);
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        if (PlayerPrefs.HasKey("DialogVolume"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("DialogVolume");
        }

        ApplyStoredVolume();
        SetMusicVolume(); // Apply the music volume to ensure sync
        SetSFXVolume();   // Apply the SFX volume to ensure sync
        SetDialogVolume();
    }


    private void ApplyStoredVolume()
    {
        float musicVolume = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1f;
        float SFXVolume = PlayerPrefs.HasKey("SFXVolume") ? PlayerPrefs.GetFloat("SFXVolume") : 1f;
        float DialogVolume = PlayerPrefs.HasKey("DialogVolume") ? PlayerPrefs.GetFloat("DialogVolume") : 1f;

        myMixer.SetFloat("Music", Mathf.Log10(musicVolume) * 20);
        myMixer.SetFloat("SFX", Mathf.Log10(SFXVolume) * 20);
        myMixer.SetFloat("Dialog", Mathf.Log10(DialogVolume) * 20);
    }
}
