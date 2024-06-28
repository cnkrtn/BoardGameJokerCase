using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : Singleton<AudioManager>
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, soundSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMusic("MenuMusic");

    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        {
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
    }
    
    public void PlaySound(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        {
            if (s == null)
            {
                Debug.Log("Sound Not Found");
            }
            else
            {
                soundSource.clip = s.clip;
                soundSource.PlayOneShot(s.clip);
            }
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute=!musicSource.mute;
    }
    
    public void ToggleSfx()
    {
        soundSource.mute=!soundSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SfxVolume(float volume)
    {
        soundSource.volume = volume;
    }
}
