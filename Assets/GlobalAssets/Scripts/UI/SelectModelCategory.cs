using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class SelectModelCategory : MonoBehaviour
    {
        private ProjectController projectController;
        void Start()
        {
            projectController = ProjectController.Instance;
        }
        public void setModelCategoryToClassical()
        {
            projectController.modelCategory = 0; //classical
        }
        public void setModelCategoryToResnet()
        {
            projectController.modelCategory = 1; //Resnet
        }
        public void setModelCategoryToCNN()
        {
            projectController.modelCategory = 2; //cnn
        }
    }
}