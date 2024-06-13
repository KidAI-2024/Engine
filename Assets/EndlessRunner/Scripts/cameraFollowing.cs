using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowing : MonoBehaviour
{

    public Transform player;
    public float offset_x;
    public float offset_y;
    public float offset_z;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position= new Vector3(0,player.position.y+offset_y, player.position.z +offset_z);
    }

    public IEnumerator shaking(float period)
    {
        Vector3 originalPos = transform.position;
        float passedTime = 0f;
        while (passedTime <period)
        {
            float x = Random.Range(-1f, 2f);
            float y = Random.Range(3f, 1f);
            transform.localPosition = new Vector3(x, originalPos.y, originalPos.z);
            passedTime+=Time.deltaTime;
            yield return null;
        }

        transform.localPosition=originalPos;
    }



}
