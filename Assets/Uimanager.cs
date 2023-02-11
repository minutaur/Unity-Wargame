using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Uimanager : MonoBehaviour
{

    public void Quitgame()
    {
        Application.Quit();
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene("Demo");
    }
}
