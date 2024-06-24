using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
namespace Karting.UI
{

    public class AudioController : MonoBehaviour
    {
        public AudioMixer audioMixer;
        bool isMuted = false;
        public float currentVolume = 0.5f;
        void Start()
        {
            audioMixer.SetFloat("Volume", 20 * Mathf.Log10(currentVolume));
        }
        public void ToggleMute()
        {
            if (isMuted)
            {
                UnmuteAudio();
            }
            else
            {
                MuteAudio();
            }
            isMuted = !isMuted;
        }
        void MuteAudio()
        {
            audioMixer.SetFloat("Volume", -80);
        }
        void UnmuteAudio()
        {
            audioMixer.SetFloat("Volume", 20 * Mathf.Log10(currentVolume));
        }
        public void ChangeVolume(float volume)
        {
            audioMixer.SetFloat("Volume", 20 * Mathf.Log10(volume));
            currentVolume = volume;
        }
    }
}