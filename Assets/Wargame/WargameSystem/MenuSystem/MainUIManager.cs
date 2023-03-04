using UnityEngine;
using UnityEngine.SceneManagement;

namespace Wargame.MenuSystem
{
    public class MainUIManager : MonoBehaviour
    {
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
