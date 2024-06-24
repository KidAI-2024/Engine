using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class SceneChanger : MonoBehaviour
    {
        public void LoadScene(string targetSceneName)
        {
            SceneManager.LoadScene(targetSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

}
