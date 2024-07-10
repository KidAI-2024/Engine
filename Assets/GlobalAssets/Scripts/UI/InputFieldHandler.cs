using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class InputFieldHandler : MonoBehaviour
    {
        public TMP_InputField inputField1; // Reference to the Input Field
        public TMP_InputField inputField2; // Reference to the Input Field
        private ProjectController projectController;
        public Text errorText1; // Reference to the Error Text component
        public Text errorText2; // Reference to the Error Text component

        void Start()
        {
            projectController = ProjectController.Instance;
            string inputFieldName1 = inputField1.name;
            string inputFieldName2 = inputField2.name;

            errorText1.text = "";
            errorText2.text = "";
            if (projectController.isTrained)
            {
                Debug.Log("input field1: " + inputFieldName1);
                Debug.Log("input field2: " + inputFieldName2);
                if (inputFieldName2 == "model_selection")
                {
                    inputField2.text = projectController.classicalModelType.ToString();
                }
                else if (inputFieldName1 == "feature_extraction")
                {
                    inputField1.text = projectController.featureExtractionTypeImg.ToString();
                }
                else if (inputFieldName1 == "epochs")
                {
                    inputField1.text = projectController.epochs.ToString();
                }
                else if (inputFieldName2 == "learning_rate")
                {
                    inputField2.text = projectController.learningRate.ToString();
                }
            }
        }
        public void emptyError1()
        {
            errorText1.text = "";
        }
        public void emptyError2()
        {
            errorText2.text = "";
        }
        // read the input field value and set the value of epochs in the project controller
        public bool ReadInputFieldEpochsResNet()
        {
            string userInput = inputField1.text; // Get the text from the Input Field
            // set the value of epochs in the project controller
            try
            {
                if (int.Parse(userInput) > 0)
                {
                    projectController.epochs = int.Parse(userInput);
                    errorText1.text = "";
                    return true;
                }
                else
                {
                    errorText1.text = "Invalid Number Of Epochs. Please enter a number greater than 0";
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                errorText1.text = "Invalid Number Of Epochs. Please enter a number greater than 0";
            }
            return false;
        }
        // read the input field value and set the value of learning rate in the project controller
        public bool ReadInputFieldLearningRateResNet()
        {
            string userInput = inputField2.text; // Get the text from the Input Field
            // set the value of learning rate in the project controller
            try
            {
                if (float.Parse(userInput) > 0)
                {
                    projectController.learningRate = float.Parse(userInput);
                    errorText2.text = "";
                    return true;
                }
                else
                {

                    errorText2.text = "Invalid Learning Rate. Please enter a number that is greater than 0";
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);

                errorText2.text = "Invalid Learning Rate. Please enter a number that is greater than 0";
            }
            Debug.Log("User Input: " + projectController.learningRate);
            return false;
        }
        public void ReadInputResNet()
        {
            bool epochsBool = ReadInputFieldEpochsResNet();
            bool lrBool = ReadInputFieldLearningRateResNet();
            if (epochsBool && lrBool)
            {
                projectController.modelCategory = 1; //Resnet
            }
        }
        // read the input field value and set the value of epochs in the project controller
        public bool ReadInputFieldEpochsCNN()
        {
            string userInput = inputField1.text; // Get the text from the Input Field
            // set the value of epochs in the project controller
            try
            {
                if (int.Parse(userInput) > 0)
                {
                    projectController.epochs = int.Parse(userInput);
                    errorText1.text = "";
                    return true;
                }
                else
                {
                    errorText1.text = "Invalid Number Of Epochs. Please enter a number greater than 0";
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                errorText1.text = "Invalid Number Of Epochs. Please enter a number greater than 0";
            }
            return false;
        }
        // read the input field value and set the value of learning rate in the project controller
        public bool ReadInputFieldLearningRateCNN()
        {
            string userInput = inputField2.text; // Get the text from the Input Field
            // set the value of learning rate in the project controller
            try
            {
                if (float.Parse(userInput) > 0)
                {
                    projectController.learningRate = float.Parse(userInput);
                    errorText2.text = "";
                    return true;
                }
                else
                {

                    errorText2.text = "Invalid Learning Rate. Please enter a number that is greater than 0";
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);

                errorText2.text = "Invalid Learning Rate. Please enter a number that is greater than 0";
            }
            Debug.Log("User Input: " + projectController.learningRate);
            return false;
        }
        public void ReadInputCNN()
        {
            bool epochsBool = ReadInputFieldEpochsCNN();
            bool lrBool = ReadInputFieldLearningRateCNN();
            if (epochsBool && lrBool)
            {
                projectController.modelCategory = 2; //CNN
            }
        }
        // read the input field value and set the value of classical model type in the project controller
        public bool ReadInputFieldClassicalModelType()
        {
            string userInput = inputField2.text; // Get the text from the Input Field
            // set the value of classical model type in the project controller
            try
            {
                if (int.Parse(userInput) >= 0 && int.Parse(userInput) <= 2)
                {
                    projectController.classicalModelType = int.Parse(userInput);
                    errorText2.text = "";
                    return true;
                }
                else
                {
                    errorText2.text = "Invalid Model Type. Please enter a number between 0 and 2";
                }
            }
            catch (System.Exception e)
            {
                errorText2.text = "Invalid Model Type. Please enter a number between 0 and 2";
            }
            Debug.Log("User Input: " + userInput);
            return false;
        }
        // read the input field value and set the value of feature extraction type in the project controller
        public bool ReadInputFieldfeatureExtractionTypeImg()
        {
            string userInput = inputField1.text; // Get the text from the Input Field
            // set the value of feature extraction type in the project controller
            // check if the input is 0, 1 or 2
            try
            {
                if (int.Parse(userInput) >= 0 && int.Parse(userInput) <= 2)
                {
                    projectController.featureExtractionTypeImg = int.Parse(userInput);
                    errorText1.text = "";
                    return true;
                }
                else
                {
                    errorText1.text = "Invalid Feature Type. Please enter a number between 0 and 2";
                }
            }
            catch (System.Exception e)
            {
                errorText1.text = "Invalid Feature Type. Please enter a number between 0 and 2";
            }
            return false;
        }
        public void ReadInputClassical()
        {
            bool featureBool = ReadInputFieldfeatureExtractionTypeImg();
            bool modelBool = ReadInputFieldClassicalModelType();
            if (featureBool && modelBool)
            {
                projectController.modelCategory = 0; //classical
            }
        }
    }
}
