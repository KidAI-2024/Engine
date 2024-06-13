using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float for_speed;
    //public Transform center_position;
    //public Transform left_position;
    //public Transform right_position;
    public float current_position;
    public float side_speed;
    private float original_force;
    public float jump_force;

    public Rigidbody rb;
   
    // Start is called before the first frame update
    void Start()
    {
        current_position = 1;
        Physics.gravity = new Vector3(0, -50f, 0);
    }
 
    private void OnTriggerEnter(Collider other)
    {


        //Debug.Log("HELLLLLLLLLLLLLLLLL");
       
        if (other.gameObject.tag == "obstacle")
        {

            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            //gameManager.instance.score++;
            //gameManager.instance.scoreText.text=gameManager.instance.score.ToString();
            gameManager.instance.eraseScore();


        }
    }
    //IEnumerable forward_jump()
    //{

    //    yield return new WaitForSeconds(5f);
    //    rb.velocity = Vector3.zero;
    //}
    // Update is called once per frame
    public IEnumerator freezeMoving(float period)
    {
        original_force = for_speed;
        for_speed = 0;
        yield return new WaitForSeconds(period);
        for_speed = original_force;


    }
    void OnDestroy()
    {
        Physics.gravity = new Vector3(0,-9.81f,0);
    }
    void Update()
    {
        

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + for_speed * Time.deltaTime);
        //transform.position += transform.forward * for_speed * Time.deltaTime;

        if (current_position == 0)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(-5.5f, transform.position.y, transform.position.z),side_speed*Time.deltaTime);

        }
        else if (current_position == 1)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, transform.position.y, transform.position.z), side_speed * Time.deltaTime);
        }
        else if (current_position == 2)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(5.5f, transform.position.y, transform.position.z), side_speed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (current_position == 0)
            {
                current_position = 1;
            }
            else if (current_position == 1)
            {
                current_position = 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (current_position == 1)
            {
                current_position = 0;
            }
            else if (current_position == 2)
            {
                current_position = 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            if (transform.position.y <= 1)
            {
                // Stop applying upward force

                rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
                rb.AddForce(transform.forward * 60, ForceMode.Impulse);
        }
    }
    }
    
}