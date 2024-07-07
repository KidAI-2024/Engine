using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GlobalAssets.UI
{
    public class LoadingPanelController : MonoBehaviour
    {
        public GameObject loadingPanel;
        

        public void ShowLoadingPanel()
        {
            loadingPanel.SetActive(true);
        }
        public void HideLoadingPanel()
        {
            loadingPanel.SetActive(false);
        }
    }
}
