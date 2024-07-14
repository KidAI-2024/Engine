/*
This script, InputFieldHandler, is designed to handle user input from various input fields in a Unity UI, 
particularly related to setting parameters for different types of machine learning models within the context of a project controller.
*/
using UnityEngine;
using TMPro; // For handling TextMeshPro input fields
using UnityEngine.UI; // For handling UI elements

namespace GlobalAssets.UI
{
    // This class handles user input from various input fields and updates the project controller accordingly
    public class InputFieldHandler : MonoBehaviour
    {
        public TMP_InputField inputField1; // Reference to the first input field
        public TMP_InputField inputField2; // Reference to the second input field
        private ProjectController projectController; // Reference to the project controller
        public Text errorText1; // Reference to the first error text component
        public Text errorText2; // Reference to the second error text component

        // Start is called before the first frame update
        void Start()
        {
            projectController = ProjectController.Instance; // Get the instance of the project controller
            string inputFieldName1 = inputField1.name; // Get the name of the first input field
            string inputFieldName2 = inputField2.name; // Get the name of the second input field

            // Initialize error texts to be empty
            errorText1.text = "";
            errorText2.text = "";

            // If the project is already trained, set the input fields with the existing project controller values
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

        // Method to clear the error text for the first input field
        public void emptyError1()
        {
            errorText1.text = "";
        }

        // Method to clear the error text for the second input field
        public void emptyError2()
        {
            errorText2.text = "";
        }

        // Method to read the input field value and set the number of epochs in the project controller for ResNet
        public bool ReadInputFieldEpochsResNet()
        {
            string userInput = inputField1.text; // Get the text from the input field
            try
            {
                // Validate and set the number of epochs
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

        // Method to read the input field value and set the learning rate in the project controller for ResNet
        public bool ReadInputFieldLearningRateResNet()
        {
            string userInput = inputField2.text; // Get the text from the input field
            try
            {
                // Validate and set the learning rate
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

        // Method to read both the epochs and learning rate input fields for ResNet and set the model category accordingly
        public void ReadInputResNet()
        {
            bool epochsBool = ReadInputFieldEpochsResNet();
            bool lrBool = ReadInputFieldLearningRateResNet();
            if (epochsBool && lrBool)
            {
                projectController.modelCategory = 1; // Set the model category to ResNet
            }
        }

        // Method to read the input field value and set the number of epochs in the project controller for CNN
        public bool ReadInputFieldEpochsCNN()
        {
            string userInput = inputField1.text; // Get the text from the input field
            try
            {
                // Validate and set the number of epochs
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

        // Method to read the input field value and set the learning rate in the project controller for CNN
        public bool ReadInputFieldLearningRateCNN()
        {
            string userInput = inputField2.text; // Get the text from the input field
            try
            {
                // Validate and set the learning rate
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

        // Method to read both the epochs and learning rate input fields for CNN and set the model category accordingly
        public void ReadInputCNN()
        {
            bool epochsBool = ReadInputFieldEpochsCNN();
            bool lrBool = ReadInputFieldLearningRateCNN();
            if (epochsBool && lrBool)
            {
                projectController.modelCategory = 2; // Set the model category to CNN
            }
        }

        // Method to read the input field value and set the classical model type in the project controller
        public bool ReadInputFieldClassicalModelType()
        {
            string userInput = inputField2.text; // Get the text from the input field
            try
            {
                // Validate and set the classical model type
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

        // Method to read the input field value and set the feature extraction type in the project controller
        public bool ReadInputFieldfeatureExtractionTypeImg()
        {
            string userInput = inputField1.text; // Get the text from the input field
            try
            {
                // Validate and set the feature extraction type
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

        // Method to read both the feature extraction type and classical model type input fields and set the model category accordingly
        public void ReadInputClassical()
        {
            bool featureBool = ReadInputFieldfeatureExtractionTypeImg();
            bool modelBool = ReadInputFieldClassicalModelType();
            if (featureBool && modelBool)
            {
                projectController.modelCategory = 0; // Set the model category to classical
            }
        }
    }
}
