using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MicController : MonoBehaviour
{
    public GameObject AudioPrefab; // Prefab of RawAudio to instantiate
    public Transform AudioContainer; // Parent transform for instantiated RawAudios
    public Transform finalAudiosContainer; // Parent transform for instantiated RawAudios
    public Button recordButton;
    public Button autoCaptureButton;
    public GameObject autoCaptureGO;
    public TextMeshProUGUI numCapturedText;

    private int autoCaptureTime = 1;
    private WebCamTexture webcamTexture;
    private TMP_Dropdown autoCaptureDDL;
    public GameObject EmptyAudio;
    private GameObject EmptyAudioCaptureAudios;
    public List<AudioClip> capturedAudios = new List<AudioClip>();

    public RawImage microphoneIcon; // Reference to the RawImage displaying microphone icon
    public TextMeshProUGUI recordButtonText; // Text to display on the record button

    private string microphone; // Name of the microphone device
    private AudioClip currentClip; // Recorded audio clip
    private AudioSource audioSource;

    void Start()
    {
        autoCaptureButton.onClick.AddListener(AutoCapture);
        audioSource = GetComponent<AudioSource>();
        recordButton.onClick.AddListener(RecordAudio);
    }

    void Update()
    {
        numCapturedText.text = "Recorded: " + capturedAudios.Count + " audios";
    }

    public void OpenCamera(GameObject outsideAudiosContainer)
    {
        finalAudiosContainer = outsideAudiosContainer.transform;
        EmptyAudio = finalAudiosContainer.parent.parent.gameObject.transform.GetChild(2).gameObject;
        EmptyAudioCaptureAudios = AudioContainer.parent.parent.gameObject.transform.GetChild(2).gameObject;
        if (audioSource!=null &&audioSource.isPlaying)
            audioSource.Stop(); // Stop any currently playing audio
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
            return;
        }
    }

    public void AutoCaptureTimeValueChanged(string value)
    {
        autoCaptureTime = int.Parse(value);
    }

    public void AutoCapture()
    {
        autoCaptureButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
        InvokeRepeating("RecordAudio", autoCaptureTime, autoCaptureTime);
        autoCaptureButton.onClick.RemoveAllListeners();
        autoCaptureButton.onClick.AddListener(StopAutoCapture);
    }

    public void StopAutoCapture()
    {
        CancelInvoke("RecordAudio");
        autoCaptureButton.GetComponentInChildren<TextMeshProUGUI>().text = "Auto record";
        autoCaptureButton.onClick.RemoveAllListeners();
        autoCaptureButton.onClick.AddListener(AutoCapture);
    }

    public void RecordAudio()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone found!");
            return;
        }

        microphone = Microphone.devices[0];
        currentClip = Microphone.Start(microphone, false, 5, 44100);
        capturedAudios.Add(currentClip); // Add captured audio to the list

        // Instantiate the prefab and set its position
        GameObject newAudioObject = Instantiate(AudioPrefab, AudioContainer);

        float spacingX = 70f;
        float spacingY = 70f;
        int maxColumns = 3;
        int row = capturedAudios.Count / maxColumns;
        int col = capturedAudios.Count % maxColumns;
        Vector3 newPosition = new Vector3(col * spacingX + 15, -row * spacingY - 10, 0);
        newAudioObject.transform.localPosition = newPosition;

        // Set the audio clip on the AudioSource component
        AudioSource newAudioSource = newAudioObject.GetComponent<AudioSource>();
        if (newAudioSource != null)
        {
            newAudioSource.clip = currentClip;
        }

        if (col == 0 && capturedAudios.Count > 8)
        {
            AudioContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                AudioContainer.GetComponent<RectTransform>().sizeDelta.x,
                AudioContainer.GetComponent<RectTransform>().sizeDelta.y + 70
            );
        }

        recordButtonText.text = "Stop Recording";
        recordButton.onClick.RemoveAllListeners();
        recordButton.onClick.AddListener(StopRecording);

        // Update the button listener to play the correct audio
        newAudioObject.GetComponent<Button>().onClick.AddListener(() => PlayAudio(newAudioSource));
    }

    public void StopRecording()
    {
        Microphone.End(microphone); // Stop recording
        recordButtonText.text = "Record"; // Update UI text
        recordButton.onClick.RemoveAllListeners();
        recordButton.onClick.AddListener(RecordAudio); // Reattach listener
    }

    public void PlayAudio(AudioSource source)
    {
        if (audioSource.isPlaying)
            audioSource.Stop(); // Stop any currently playing audio

        audioSource.clip = source.clip; // Assign the audio clip
        audioSource.Play(); // Play the audio clip
    }

    public void CloseCamera()
    {
        if (Microphone.IsRecording(microphone))
        {
            Microphone.End(microphone);
        }

        foreach (Transform child in finalAudiosContainer)
        {
            Destroy(child.gameObject);
        }

        int i = 0;
        foreach (AudioClip clip in capturedAudios)
        {
            GameObject newAudioObject = Instantiate(AudioPrefab, finalAudiosContainer);

            float spacingX = 70f;
            float spacingY = 70f;
            int maxColumns = 3;
            int row = i / maxColumns;
            int col = i % maxColumns;

            Vector3 newPosition = new Vector3(col * spacingX + 5, -row * spacingY - 5, 0);
            newAudioObject.transform.localPosition = newPosition;

            if (col == 0 && capturedAudios.Count > 8)
            {
                finalAudiosContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    finalAudiosContainer.GetComponent<RectTransform>().sizeDelta.x,
                    finalAudiosContainer.GetComponent<RectTransform>().sizeDelta.y + 70
                );
            }

            AudioSource newAudioSource = newAudioObject.GetComponent<AudioSource>();
            if (newAudioSource != null)
            {
                newAudioSource.clip = clip;
            }

            // Update the button listener to play the correct audio
            newAudioObject.GetComponent<Button>().onClick.AddListener(() => PlayAudio(newAudioSource));

            i++;
        }

        EmptyAudio.SetActive(capturedAudios.Count == 0);
    }

    public void InstantiateCapturedAudios()
    {
        int i = 0;
        Debug.Log("Instantiate");
        foreach (AudioClip audioClip in capturedAudios)
        {
            Debug.Log("In foreach");
            GameObject newAudioObject = Instantiate(AudioPrefab, AudioContainer);

            float spacingX = 70f;
            float spacingY = 70f;
            int maxColumns = 3;
            int row = i / maxColumns;
            int col = i % maxColumns;

            Vector3 newPosition = new Vector3(col * spacingX + 15, -row * spacingY - 10, 0);
            newAudioObject.transform.localPosition = newPosition;
            newAudioObject.GetComponent<RemoveAudios>().ImageIndex = i;

            AudioSource newAudioSource = newAudioObject.GetComponent<AudioSource>();
            if (newAudioSource != null)
            {
                newAudioSource.clip = audioClip;
            }

            if (col == 0 && capturedAudios.Count > 8)
            {
                AudioContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    AudioContainer.GetComponent<RectTransform>().sizeDelta.x,
                    AudioContainer.GetComponent<RectTransform>().sizeDelta.y + 70
                );
            }

            // Update the button listener to play the correct audio
            newAudioObject.GetComponent<Button>().onClick.AddListener(() => PlayAudio(newAudioSource));

            i++;
        }
    }

}
