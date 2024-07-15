using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class NewVideoPlayer : MonoBehaviour
{
    public RawImage image;
    public VideoClip videoToPlay;
    public Button playButton;
    public Button pauseButton;
    public Slider videoSlider;

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private bool firstRun = true;
    private bool isDraggingSlider = false;
    private Color initial = Color.white;
    void Start()
    {
        playButton.onClick.AddListener(PlayVideo);
        pauseButton.onClick.AddListener(PauseVideo);
        videoSlider.onValueChanged.AddListener(SliderValueChanged);
        initial = image.color;
        // Initially set the pause button to be inactive
        pauseButton.gameObject.SetActive(false);

        // Add Event Triggers to the slider for drag handling
        AddEventTriggersToSlider();
    }

    private void AddEventTriggersToSlider()
    {
        EventTrigger trigger = videoSlider.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entryPointerDown.callback.AddListener((eventData) => { OnSliderPointerDown(); });
        trigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entryPointerUp.callback.AddListener((eventData) => { OnSliderPointerUp(); });
        trigger.triggers.Add(entryPointerUp);
    }

    IEnumerator PlayVideoCoroutine()
    {
        firstRun = false;

        // Add VideoPlayer to the GameObject
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        // Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();

        // Disable Play on Awake for both Video and Audio
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        audioSource.Pause();

        // We want to play from video clip not from url
        videoPlayer.source = VideoSource.VideoClip;

        // Set Audio Output to AudioSource
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        // Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Set video To Play then prepare Audio to prevent Buffering
        videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        // Wait until video is prepared
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        Debug.Log("Done Preparing Video");

        // Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayer.texture;

        // Play Video
        videoPlayer.Play();

        // Play Sound
        audioSource.Play();

        // Update the slider maximum value
        videoSlider.maxValue = (float)videoPlayer.clip.length;

        Debug.Log("Playing Video");
        while (videoPlayer.isPlaying)
        {
            if (!isDraggingSlider)
            {
                videoSlider.value = (float)videoPlayer.time;
            }
            Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }

        Debug.Log("Done Playing Video");

        // Re-enable the play button after the video finishes
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void PlayVideo()
    {
        if (firstRun)
        {
            image.color = Color.white;
            StartCoroutine(PlayVideoCoroutine());
        }
        else
        {
            // Make sure to reassign the texture each time
            if (videoPlayer == null)
            {
                StartCoroutine(PlayVideoCoroutine());
            }
            else
            {
                videoPlayer.Play();
                audioSource.Play();
                image.texture = videoPlayer.texture; // Reassign the texture
            }
        }

        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
        audioSource.Pause();

        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void SliderValueChanged(float value)
    {
        if (videoPlayer == null || !isDraggingSlider)
            return;

        videoPlayer.time = value;
    }

    public void OnSliderPointerDown()
    {
        isDraggingSlider = true;
    }

    public void OnSliderPointerUp()
    {
        isDraggingSlider = false;
        videoPlayer.time = videoSlider.value;
    }

    void Update()
    {
        if (videoPlayer != null && videoPlayer.isPlaying && !isDraggingSlider)
        {
            videoSlider.value = (float)videoPlayer.time;
        }
    }
    public void disabeling()
    {
        firstRun = true;
        image.color = initial;
        pauseButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
    }
   
}
