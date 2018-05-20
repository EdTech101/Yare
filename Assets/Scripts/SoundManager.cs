using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<SoundManager>();
            }
            return instance;
        }
    }

    public AudioSource MusicSource
    {
        get
        {
            return musicSource;
        }

        set
        {
            musicSource = value;
        }
    }

    [SerializeField]
    private AudioSource musicPauseSource;

    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Slider musicSlider;

    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    // Use this for initialization
    void Start()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio") as AudioClip[];

        foreach (AudioClip clip in clips)
        {
            audioClips.Add(clip.name, clip);
        }

        LoadVolume();

        musicSlider.onValueChanged.AddListener(delegate { UpdateVolume();});
        sfxSlider.onValueChanged.AddListener(delegate { UpdateVolume(); });
    }

    public void PlaySfx(string name)
    {
        sfxSource.PlayOneShot(audioClips[name]);
    }

    public void PauseMusic()
    {
        MusicSource.Pause();
        musicPauseSource.Play();
    }

    public void ContinueMusic()
    {
        MusicSource.Play();
        musicPauseSource.Pause();
    }


    public void UpdateVolume()
    {
        MusicSource.volume = musicSlider.value;
        sfxSource.volume = sfxSlider.value;

        PlayerPrefs.SetFloat("SFX", sfxSlider.value);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }

    public void LoadVolume()
    {
        MusicSource.volume = PlayerPrefs.GetFloat("Music", 0.5f); ;
        sfxSource.volume = PlayerPrefs.GetFloat("SFX", 0.5f);
        musicSlider.value = MusicSource.volume;
        sfxSlider.value = sfxSource.volume;
    }
}
