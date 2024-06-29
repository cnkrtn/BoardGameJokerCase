using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeAnimation : MonoBehaviour
{
    public void NewGameButtonPressed()
    {
        StartCoroutine(FadeOutAndChangeScene());
    }

    public void ContinueButtonPressed()
    {
        StartCoroutine(FadeOutAndChangeScene());
    }

    private IEnumerator FadeOutAndChangeScene()
    {
        
        AudioManager.Instance.FadeOutMusic(3.0f);

        yield return new WaitForSeconds(3.5f);

       
        // DataManager.Instance.LoadData();
        SceneManager.LoadScene(1);
    }
    
    public void FadeIn()
    {
        AudioManager.Instance.FadeInMusic(.7f,3.0f);
        
        
    }
}