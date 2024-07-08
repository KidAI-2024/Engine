using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    // Start is called before the first frame update
    
    public float rotationSpeed=1f;
    public AudioSource audioSource;
    public AudioClip clip;

    private void OnTriggerEnter(Collider other)
    {

          
       
        if (other.gameObject.tag == "Player")
        {
           
            transform.SetParent(null);

            //gameManager.instance.score++;
            //gameManager.instance.scoreText.text=gameManager.instance.score.ToString();
            gameManager.instance.incrementScore();
            audioSource.clip=clip;
            audioSource.Play();
            //Destroy(gameObject);
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(DelayedDestroy(clip.length));
        }
    }
    IEnumerator DelayedDestroy(float delay)
    {
        // Wait for the duration of the sound clip
        yield return new WaitForSeconds(delay);

        // Destroy the coin object
        Destroy(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 1f, 0);
        
    }
}
