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
    private GlobalAssets.Socket.SocketUDP socketClient;
    private ProjectController projectController;


    //public int pythonPredictedClass { get; private set; } = -1;
    //public string UnityPredictedClass { get; private set; } = "";
    // Start is called before the first frame update
    void Start()
    {
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        projectController = ProjectController.Instance;
        current_position = 1;
        Physics.gravity = new Vector3(0, -50f, 0);

        
        // Check if the device supports microphone
        Dictionary<string, string> message = new Dictionary<string, string>
        {

            { "event", "predict_audio" }
        };

        // Send the message to the server
        socketClient.SendMessage(message);
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
        Dictionary<string, string> message = new Dictionary<string, string>
        {

            { "event", "stop_prediction" }
        };

        // Send the message to the server
        socketClient.SendMessage(message);
    }
    string PythonToUnityClassName(string message)
    {
        if (projectController.PythonClassesToUnityClassesMap.ContainsKey(message))
        {
            return projectController.PythonClassesToUnityClassesMap[message];
        }
        return message;
    }
    void Update()
    {
        string pred_class = "";
        string pred_control = "";
        // Receive the response from the server (Python)
        if (socketClient.isDataAvailable())
        {
            Dictionary<string, string> response = socketClient.ReceiveDictMessage();

            if (response["event"] == "predict_audio")
            {
                string pred = response["prediction"];
                if (pred != "none")
                {       pred_class = PythonToUnityClassName(pred);
                    if(pred_class!="none")
                pred_control = projectController.classesToControlsMap[pred_class];
            }
                //Debug.Log("Received Prediction: " + MapToClassName(pred) + " " + pred);
                //predictionText.text = MapToClassName(pred);
            }
           
        }


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
        if (Input.GetKeyDown(KeyCode.RightArrow)|| pred_control == "Right")
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
        else if (Input.GetKeyDown(KeyCode.LeftArrow)|| pred_control == "Left")
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
        else if (Input.GetKeyDown(KeyCode.UpArrow)|| pred_control == "Jump")
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