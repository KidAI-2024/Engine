using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class playerMovement : MonoBehaviour
{
    public float for_speed;
    public float current_position;
    public float side_speed;
    private float original_force;
    public float jump_force;
    public string predicted_control;
    public Vector3 initial_po;
    public Rigidbody rb;
    public float right_off;
    public float left_off;
    private GlobalAssets.Socket.SocketUDP socketClient;
    private ProjectController projectController;
    private bool start_run = false;
    public float blinkDuration = 0.1f; // Duration for which the player is invisible
    public float blinkInterval = 0.1f; // Interval between each blink
    public int blinkCount = 2; // Number of times to blink
    private Animator animator;
    private Renderer[] renderers;
    //public bool is_jumping_up = false;
    void Start()
    {
      
        socketClient = GlobalAssets.Socket.SocketUDP.Instance;
        projectController = ProjectController.Instance;
        current_position = 1;
        Physics.gravity = new Vector3(0, -9.8f, 0);
        transform.position = initial_po;
        predicted_control = "";
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "obstacle")
        {
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            other.gameObject.SetActive(false);
            gameManager.instance.eraseScore();
        }
    }
  

    public IEnumerator BlinkPlayerRoutine()
    {
        Vector3 scale_original=gameObject.transform.localScale;
        for (int i = 0; i < blinkCount; i++)
        {
            gameObject.transform.localScale = Vector3.zero;
           
            yield return new WaitForSeconds(blinkDuration);

            gameObject.transform.localScale = scale_original;
            // Restore the animation state
           

            // Wait for the interval
            yield return new WaitForSeconds(blinkInterval);
        }
    }
    public IEnumerator freezeMoving(float period)
    {
        original_force = for_speed;
        for_speed = 0;
        yield return new WaitForSeconds(period);
        for_speed = original_force;
    }

    void OnDestroy()
    {
        Debug.Log("stop prediction");
        Physics.gravity = new Vector3(0, -9.81f, 0);
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

    void FixedUpdate()
    {
        if (start_run)
        {
            transform.position += new Vector3(0, 0, for_speed * Time.fixedDeltaTime);
        }
        else
        {
            transform.position = initial_po;
        }
    }

    void Update()
    {
        string pred_class = "";
        string pred_control = "";
        predicted_control = "";
        if (socketClient.isDataAvailable())
        {

            Dictionary<string, string> response = socketClient.ReceiveDictMessage();

            if (response["event"] == "predict_audio")
            {

                string pred = response["prediction"];
                //Debug.Log("prediction " + pred);
                pred_class = PythonToUnityClassName(pred);
                //Debug.Log("prediction class map" + pred_class);
                if (pred_class != "none")
                    pred_control = projectController.classesToControlsMap[pred_class];
                Debug.Log("prediction zction map" + pred_control);
                predicted_control = pred_control;
                gameManager.instance.change_class_ui(predicted_control);
                //Debug.Log("Received Prediction: " + MapToClassName(pred) + " " + pred);
                //predictionText.text = MapToClassName(pred);
            }

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            start_run = true;
            Dictionary<string, string> message = new Dictionary<string, string>
        {

            { "event", "predict_audio" }
        };

            // Send the message to the server
            socketClient.SendMessage(message);
        }
        
        if (start_run)
        {
            UpdatePosition();
            HandleInput();
        }
    }

    private void UpdatePosition()
    {
        Vector3 targetPosition = initial_po;

        if (current_position == 0)
        {
            targetPosition.x -= left_off;
        }
        else if (current_position == 2)
        {
            targetPosition.x += right_off;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), side_speed * Time.deltaTime);
    }

    private void HandleInput()
    {
        //if (transform.position.y < 0.2) is_jumping_up = false;
        if (Input.GetKeyDown(KeyCode.RightArrow) || predicted_control == "Right")
        {
            if (current_position < 2)
            {
                current_position++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || predicted_control == "Left")
        {
            if (current_position > 0)
            {
                current_position--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || predicted_control == "Jump")
        {
            if (transform.position.y < 0.5)
            {
                rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
                rb.AddForce(transform.forward * 60, ForceMode.Impulse);
                //is_jumping_up = true;
                //predicted_control = "";
            }
        }
    }
}
