using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static gameManager instance;
    public  int score;
    public AudioSource audioSource;
    public AudioClip clip;
    public cameraFollowing cam;
    public playerMovement player;
    public TextMeshProUGUI scoring;
    private float originalFogDensity;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        originalFogDensity=RenderSettings.fogDensity;
    }
    public void incrementScore()
    {
        score++;
        scoring.text ="Score : " +score;
    }
     public void eraseScore()
    {
        score=0;
        scoring.text ="Score : " +score;
        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
        StartCoroutine(player.freezeMoving(0.5f));
        StartCoroutine(cam.shaking(0.5f));
        StartCoroutine(changeFog());
    }


    IEnumerator changeFog()
    {
        RenderSettings.fogDensity = 0.25f;
        yield return new WaitForSeconds(0.8f);
        RenderSettings.fogDensity = originalFogDensity;
    }



// Update is called once per frame
void Update()
    {
        
    }
}
