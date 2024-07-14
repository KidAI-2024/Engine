using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioConfigModel : MonoBehaviour
{
    public Button extraction1;
    public Button extraction2;
    public Button extraction3;
    public Button model1;
    public Button model2;
    public Button model3;
    public Button config1;
    public Button config2;
    public Button config3;

    public GameObject label_1;
    public GameObject label_2;

    public Button nextButton;
    public Button saveButton;
    private ProjectController ProjectController;
    public List<string> selectedFeats = new List<string>();
    public string selectedModel;
    public string selectedConfig;
    public GameObject dialog; // Reference to your dialog GameObject
    void Start()
    {
        // Add listeners to the feature extraction buttons
        extraction1.onClick.AddListener(() => ToggleFeat("mfccs", extraction1));
        extraction2.onClick.AddListener(() => ToggleFeat("chroma", extraction2));
        extraction3.onClick.AddListener(() => ToggleFeat("mel", extraction3));

        nextButton.onClick.AddListener(OnNextPressed);
        saveButton.onClick.AddListener(OnSavePressed);
        ProjectController = ProjectController.Instance;
    }

    void ToggleFeat(string feat, Button button)
    {
        if (selectedFeats.Contains(feat))
        {
            selectedFeats.Remove(feat);
            button.GetComponent<RawImage>().color = Color.white; // Unselect the button
        }
        else
        {
            selectedFeats.Add(feat);
            
            button.GetComponent<RawImage>().color = Color.green; // Select the button
        }
        Debug.Log("Selected Features: " + string.Join(", ", selectedFeats));
    }

    void setModel(string model, Button selectedButton)
    {
        selectedModel = model;
        Debug.Log("Selected Model: " + selectedModel);
        UpdateConfigButtons();

        // Deselect all model buttons
        DeselectAllModelButtons();

        // Select the clicked button
        selectedButton.GetComponent<RawImage>().color= Color.green; // Change color to indicate selection
    }

    void setConfig(string config, Button selectedButton)
    {
        selectedConfig = config;
        Debug.Log("Selected Config: " + selectedConfig);

        // Deselect all config buttons
        DeselectAllConfigButtons();

        // Select the clicked button
        selectedButton.GetComponent<RawImage>().color= Color.green; // Change color to indicate selection
    }

    void DeselectAllModelButtons()
    {
        model1.GetComponent<RawImage>().color = Color.white;
        model2.GetComponent<RawImage>().color = Color.white;
        model3.GetComponent<RawImage>().color = Color.white;
       
    }

    void DeselectAllConfigButtons()
    {
        config1.GetComponent<RawImage>().color = Color.white;
        config2.GetComponent<RawImage>().color = Color.white;
        config3.GetComponent<RawImage>().color = Color.white;
    }

    void UpdateConfigButtons()
    {
        // Clear previous listeners
        config1.onClick.RemoveAllListeners();
        config2.onClick.RemoveAllListeners();
        config3.onClick.RemoveAllListeners();

        if (selectedModel == "svm")
        {
            SetButtonText(config1, "Linear");
            SetButtonText(config2, "RBF");
            SetButtonText(config3, "Poly");

            label_2.GetComponent<TextMeshProUGUI>().text = "Kernels";
            config1.onClick.AddListener(() => setConfig("linear", config1));
            config2.onClick.AddListener(() => setConfig("rbf", config2));
            config3.onClick.AddListener(() => setConfig("poly", config3));
        }
        else if (selectedModel == "knn")
        {
            SetButtonText(config1, "1");
            SetButtonText(config2, "3");
            SetButtonText(config3, "5");

            label_2.GetComponent<TextMeshProUGUI>().text = "Nearest Neighbors";
            config1.onClick.AddListener(() => setConfig("1", config1));
            config2.onClick.AddListener(() => setConfig("3", config2));
            config3.onClick.AddListener(() => setConfig("5", config3));
        }
        else if (selectedModel == "boost")
        {
            SetButtonText(config1, "50");
            SetButtonText(config2, "100");
            SetButtonText(config3, "200");

            label_2.GetComponent<TextMeshProUGUI>().text = "Estimators";
            config1.onClick.AddListener(() => setConfig("50", config1));
            config2.onClick.AddListener(() => setConfig("100", config2));
            config3.onClick.AddListener(() => setConfig("200", config3));
        }
    }

    void SetButtonText(Button button, string text)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        else
        {
            Debug.LogError("No TextMeshProUGUI component found in the Button's children.");
        }
    }

    public void OnNextPressed()
    {
        // Change components visibility
        extraction1.gameObject.SetActive(false);
        extraction2.gameObject.SetActive(false);
        extraction3.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        model1.gameObject.SetActive(true);
        model2.gameObject.SetActive(true);
        model3.gameObject.SetActive(true);

        model1.onClick.AddListener(() => setModel("svm", model1));
        model2.onClick.AddListener(() => setModel("knn", model2));
        model3.onClick.AddListener(() => setModel("boost", model3));

        config1.gameObject.SetActive(true);
        config2.gameObject.SetActive(true);
        config3.gameObject.SetActive(true);

        label_1.SetActive(true);
        label_2.SetActive(true);
        saveButton.gameObject.SetActive(true);
    }

    public void OnSavePressed()
    {
        extraction1.gameObject.SetActive(true);
        extraction2.gameObject.SetActive(true);
        extraction3.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);

        model1.gameObject.SetActive(false);
        model2.gameObject.SetActive(false);
        model3.gameObject.SetActive(false);
        config1.gameObject.SetActive(false);
        config2.gameObject.SetActive(false);
        config3.gameObject.SetActive(false);

        label_1.SetActive(false);
        label_2.SetActive(false);
        saveButton.gameObject.SetActive(false);
        // Save the current state
        dialog.SetActive(false);
        ProjectController.audioModel = selectedModel;
        ProjectController.audioConfig = selectedConfig;
        ProjectController.audioFeats = string.Join("_", selectedFeats);

        Debug.Log($"Saved State - Features: {string.Join(", ", selectedFeats)}, Model: {selectedModel}, Config: {selectedConfig}");
    }
}
