using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class SceneChanger : MonoBehaviour
    {
        public void LoadScene(string targetSceneName)
        {
            try{
                SceneManager.LoadScene(targetSceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading scene: " + e.Message);
            }
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

}
