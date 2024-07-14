using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class videoPlayer : MonoBehaviour
{
    public VideoPlayer vid;
    public Button playButton;
    public Button pauseButton;
    public Slider videoSlider;

    void Start()
    {
        vid = GetComponent<VideoPlayer>();
        playButton.onClick.AddListener(PlayVideo);
        pauseButton.onClick.AddListener(PauseVideo);
        videoSlider.onValueChanged.AddListener(SliderSeek);

        // Disable controls until video is prepared
        playButton.interactable = false;
        pauseButton.interactable = false;
        videoSlider.interactable = false;

        if (vid.clip != null)
        {
            vid.playbackSpeed = 1.0f; // Set initial playback speed
            vid.skipOnDrop = true; // Skip frames if the video can't keep up
            StartCoroutine(PrepareVideo());
        }
    }

    IEnumerator PrepareVideo()
    {
        vid.Prepare();
        while (!vid.isPrepared)
        {
            yield return null; // Wait until the video is prepared
        }
        VideoPrepared(vid);
    }

    void VideoPrepared(VideoPlayer vp)
    {
        videoSlider.maxValue = (float)vp.length;
        vid.Play();
        vid.Pause();

        // Enable controls after video is prepared
        playButton.interactable = true;
        pauseButton.interactable = true;
        videoSlider.interactable = true;
    }

    void Update()
    {
        if (vid.isPlaying)
        {
            videoSlider.value = (float)vid.time;
        }
    }

    void PlayVideo()
    {
        if (vid.clip != null)
        {
            vid.Play();
        }
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