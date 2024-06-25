
using UnityEngine;
using UnityEngine.Serialization;


public class SwitchCameras : MonoBehaviour
{
    private bool _isChar = true;
    [SerializeField] private GameObject charCam, godCam;
    

    void Update()
    {
      SwitchCamera();
        
    }

    private void SwitchCamera()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (_isChar)
            {
                _isChar = !_isChar;
                charCam.SetActive(false);
                godCam.SetActive(true);
               
            }
            else
            {
                _isChar = !_isChar;
                charCam.SetActive(true);
                godCam.SetActive(false);
                
            }
        }
    }
}
