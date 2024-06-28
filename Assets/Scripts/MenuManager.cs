using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void NewGameButtonPressed()
    { 
        // DataManager.Instance.isNewGame = true;
        // DataManager.Instance.LoadData();
        SceneManager.LoadScene(1);
    }
    
    public void ContinueButtonPressed()
    { 
        DataManager.Instance.isNewGame = false;
        DataManager.Instance.LoadData();
       // SceneManager.LoadScene(1);
    }
}
