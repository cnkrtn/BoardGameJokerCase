using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : Singleton<AudioManager>
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource;
    public AudioSource[] soundSources = new AudioSource[5]; // Array of AudioSources for sound effects
    public Transform soundSourceObject;

    private void Awake()
    {
        base.Awake();

        // Add and initialize AudioSource components
        for (int i = 0; i < soundSources.Length; i++)
        {
            soundSources[i] = soundSourceObject.gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        
        MusicVolume(DataManager.Instance.musicVolume);
        SfxVolume(DataManager.Instance.soundVolume);
        
        PlayMusic("MenuMusic");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            AudioSource availableSource = GetAvailableAudioSource();
            if (availableSource != null)
            {
                availableSource.PlayOneShot(s.clip);
            }
            else
            {
                Debug.Log("No available AudioSource to play sound");
            }
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in soundSources)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }
        Debug.Log("No available AudioSource found");
        return null; 
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
        
    }

    public void ToggleSfx()
    {
        foreach (AudioSource source in soundSources)
        {
            source.mute = !source.mute;
          
        }
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    
    }

    public void SfxVolume(float volume)
    {
        foreach (AudioSource source in soundSources)
        {
            source.volume = volume;
        }
      
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutCoroutine(musicSource, duration));
    }

    public void FadeInMusic(float targetVolume, float duration)
    {
        StartCoroutine(FadeInCoroutine(musicSource, targetVolume, duration));
    }

    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 0;
    }

    private IEnumerator FadeInCoroutine(AudioSource audioSource, float targetVolume, float duration)
    {
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
