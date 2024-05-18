using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MortalKombat
{
    public class Sound : MonoBehaviour
    {
        AudioSource audioSource;
        public AudioClip whoofSound;
        public AudioClip headHitSound;
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayWhoofSound()
        {
            audioSource.PlayOneShot(whoofSound);
        }

        public void PlayHeadHitSound()
        {
            audioSource.PlayOneShot(headHitSound);
        }
    }
}