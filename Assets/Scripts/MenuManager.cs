using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider, soundSlider;
    [SerializeField] private Toggle musicToggle, soundToggle;
    [SerializeField] private Animator fadeAnimation;
    private void Start()
    {
        SetVolumeSliders();
        AudioManager.Instance.PlayMusic("MenuMusic2");
    }

    public void NewGameButtonPressed()
    { 
        DataManager.Instance.isNewGame = true;
        DataManager.Instance.LoadData();
        fadeAnimation.Play("FadeAnimation");
         
    }
    
    public void ContinueButtonPressed()
    { 
        DataManager.Instance.isNewGame = false;
        DataManager.Instance.LoadData();
        fadeAnimation.Play("FadeAnimationContinue");
    }

    public void SettingButtonPressed(RectTransform panel)
    {
        panel.localScale = Vector3.zero;
        panel.gameObject.SetActive(true);
        StartCoroutine(LerpHelper.LerpScale(panel, panel.localScale, Vector3.one * 0.8f, .2f,
            LerpHelper.GetEasingFunction(EasingFunctionType.Linear)));
    }
    
    public void CloseButtonPressed(RectTransform panel)
    {
        panel.localScale = Vector3.one * .8f;
        StartCoroutine(LerpHelper.LerpScale(panel, panel.localScale, Vector3.zero, .2f,
            LerpHelper.GetEasingFunction(EasingFunctionType.Linear)));

        DataManager.Instance.SaveData();
    }
    
    public void ToggleMusic(){AudioManager.Instance.ToggleMusic();}
    public void ToggleSound(){AudioManager.Instance.ToggleSfx();}
    public void MusicVolume(){AudioManager.Instance.MusicVolume(musicSlider.value);}
    public void SfxVolume(){AudioManager.Instance.SfxVolume(soundSlider.value);}

    private void SetVolumeSliders()
    {
        musicSlider.value = DataManager.Instance.musicVolume;
        soundSlider.value = DataManager.Instance.soundVolume;
        musicToggle.isOn = DataManager.Instance.musicToggle;
        soundToggle.isOn=DataManager.Instance.soundToggle;
    }
   
}
