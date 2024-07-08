using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GlobalAssets.UI
{
    public class InputFieldHandler : MonoBehaviour
    {
        public TMP_InputField inputField; // Reference to the Input Field
        private ProjectController projectController;
        public Text errorText; // Reference to the Error Text component

        void Start()
        {
            projectController = ProjectController.Instance;
            emptyErrorText();
        }
        public void emptyErrorText()
        {
            errorText.text = "";
        }
        // read the input field value and set the value of epochs in the project controller
        public void ReadInputFieldEpochs()
        {
            string userInput = inputField.text; // Get the text from the Input Field
            // set the value of epochs in the project controller
            try
            {
                if (int.Parse(userInput) > 0)
                {
                    projectController.epochs = int.Parse(userInput);
                    emptyErrorText();

                }
                else
                {
                    errorText.text = "Invalid Number Of Epochs. Please enter a number greater than 0";
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
                errorText.text = "Invalid Number Of Epochs. Please enter a number greater than 0";
            }
            Debug.Log("User Input: " + projectController.epochs);
        }
        // read the input field value and set the value of learning rate in the project controller
        public void ReadInputFieldLearningRate()
        {
            string userInput = inputField.text; // Get the text from the Input Field
            // set the value of learning rate in the project controller
            try
            {
                if (float.Parse(userInput) > 0)
                {
                    projectController.learningRate = float.Parse(userInput);
                    emptyErrorText();

                }
                else
                {

                    errorText.text = "Invalid Learning Rate. Please enter a number that is greater than 0";
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);

                errorText.text = "Invalid Learning Rate. Please enter a number that is greater than 0";
            }
            Debug.Log("User Input: " + projectController.learningRate);
        }
        // read the input field value and set the value of classical model type in the project controller
        public void ReadInputFieldClassicalModelType()
        {
            string userInput = inputField.text; // Get the text from the Input Field
            // set the value of classical model type in the project controller
            try
            {
                if (int.Parse(userInput) >= 0 && int.Parse(userInput) <= 2)
                {
                    projectController.classicalModelType = int.Parse(userInput);
                    emptyErrorText();

                }
                else
                {
                    errorText.text = "Invalid Model Type. Please enter a number between 0 and 2";
                }
            }
            catch (System.Exception e)
            {
                errorText.text = "Invalid Model Type. Please enter a number between 0 and 2";
            }
            Debug.Log("User Input: " + userInput);
        }
        // read the input field value and set the value of feature extraction type in the project controller
        public void ReadInputFieldfeatureExtractionTypeImg()
        {
            string userInput = inputField.text; // Get the text from the Input Field
            // set the value of feature extraction type in the project controller
            // check if the input is 0, 1 or 2
            try
            {
                if (int.Parse(userInput) >= 0 && int.Parse(userInput) <= 2)
                {
                    projectController.featureExtractionTypeImg = int.Parse(userInput);
                    emptyErrorText();

                }
                else
                {
                    errorText.text = "Invalid Feature Type. Please enter a number between 0 and 2";
                }
            }
            catch (System.Exception e)
            {
                errorText.text = "Invalid Feature Type. Please enter a number between 0 and 2";
            }
        }

    }
}
