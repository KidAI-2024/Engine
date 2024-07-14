using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class videoPlayer : MonoBehaviour
{
    public VideoPlayer vid;
    public Button playButton;
    public Button pauseButton;
    public Slider videoSlider;

    bool videoPrepared = false;

    void Start()
    {
        vid = GetComponent<VideoPlayer>();
        playButton.onClick.AddListener(PlayVideo);
        pauseButton.onClick.AddListener(PauseVideo);
        videoSlider.onValueChanged.AddListener(SliderSeek);

        playButton.interactable = false;
        pauseButton.interactable = false;
        videoSlider.interactable = false;

        if (vid.clip != null)
        {
            // Set initial playback speed
            vid.playbackSpeed = 1.0f;

            // Skip frames if the video can't keep up
            vid.skipOnDrop = true;

            // Set slider's maxValue to video length
            videoSlider.maxValue = (float)vid.clip.length;

            // Ensure slider doesn't reach the end prematurely
            videoSlider.value = 0.0f;

            // Enable controls
            playButton.interactable = true;
            pauseButton.interactable = true;
            videoSlider.interactable = true;

            // Listen for video prepared event
            vid.prepareCompleted += VideoPrepared;
            vid.Prepare();
        }
    }

    void VideoPrepared(VideoPlayer vp)
    {
        // Video prepared callback
        videoPrepared = true;

        // Play the video once prepared
        vid.Play();
    }

    void Update()
    {
        if (videoPrepared && vid.isPlaying)
        {
            videoSlider.value = (float)vid.time;
        }
    }

    void PlayVideo()
    {
        if (vid.clip != null)
        {
            if (!videoPrepared)
            {
                // If video is not prepared, prepare and play on completion
                vid.prepareCompleted += PlayVideoOnPrepareComplete;
                vid.Prepare();
            }
            else
            {
                // Play the video if already prepared
                vid.Play();
            }
        }
    }

    void PlayVideoOnPrepareComplete(VideoPlayer vp)
    {
        // Play video once prepared
        vid.prepareCompleted -= PlayVideoOnPrepareComplete;
        vid.Play();
    }

    void PauseVideo()
    {
        if (vid.clip != null)
        {
            vid.Pause();
        }
    }

    void SliderSeek(float value)
    {
        if (vid.clip != null)
        {
            vid.time = value;
        }
    }
}