/*
    Script Name: PlayerController

    Purpose:
    This script manages player control, including movement, camera control, footstep sounds, zoom functionality, and scoring for correctly classified images in a game environment.

    Public Variables:
    - public Camera playerCamera: Reference to the player's camera.
    - public float walkingSpeed: Speed of the player's walking movement.
    - public float runingSpeed: Speed of the player's running movement.
    - public float jumpingPower: Power of the player's jump (not currently used).
    - public float gravity
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Survival
{
    // This script ensures a CharacterController component is added to the GameObject automatically if not already present.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // Public Variables

        // Reference to the player's camera
        public Camera playerCamera;

        // Movement Settings:
        public float walkingSpeed = 5f; // Speed of walking movement
        public float runingSpeed = 10f; // Speed of running movement
        public float jumpingPower = 5f; // Power of jump (not currently used)
        public float gravityForce = 10f; // Gravity force applied to the player

        // Camera Settings:
        public float lookingSpeed = 2f; // Speed of camera rotation
        public float lookInXLimit = 75f; // Limit for camera rotation on the X-axis
        public float cameraRotationSmoothness = 5f; // Smoothness of camera rotation

        // Ground Sounds:
        public AudioClip[] woodFootStepSoundsArr; // Footstep sounds on wood
        public AudioClip[] tileFootStepSoundsArr; // Footstep sounds on tile
        public AudioClip[] carpetFootStepSoundsArr; // Footstep sounds on carpet
        public Transform footStepAudioPosition; // Position for playing footstep sounds
        public AudioSource audioSource; // AudioSource component for playing sounds

        // Score
        public Text score;

        // Camera Zoom Settings:
        public int zoomFieldOfView = 35; // Field of view when zoomed in
        public int initialFieldOfView; // Initial field of view
        public float cameraZoomSmoothness = 1; // Smoothness of camera zoom
        // Number of correctly classified images
        public int numOfCorrectClassifiedImgs = 0;

        // Private Variables
        Vector3 moveDirection = Vector3.zero; // Movement direction
        float rotationX = 0; // X-axis rotation for camera
        float rotationY = 0; // Y-axis rotation for camera
        private bool isZoomed = false; // Flag to check if camera is zoomed in
        private bool canMove = true; // Flag to check if the player can move
        private bool isWalking = false; // Flag to check if the player is walking
        private bool isFootstepCoroutineRunning = false; // Flag to check if the footstep coroutine is running
        private AudioClip[] currentFootStepSounds; // Array to hold current footstep sounds

        CharacterController characterController; // Reference to the CharacterController component

        // Static instance of the PlayerController for singleton pattern
        private static PlayerController instance;

        // Property to access the singleton instance of PlayerController
        public static PlayerController Instance
        {
            get
            {
                if (instance == null)
                {
                    // Find the instance in the scene or create a new one if not found
                    instance = FindObjectOfType<PlayerController>();
                    if (instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(PlayerController).Name);
                        instance = singleton.AddComponent<PlayerController>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return instance;
            }
        }

        // Initialization method called when the script instance is being loaded
        void Awake()
        {
            // Ensure there is only one instance of PlayerController and it persists between scenes
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Initialization method called at the start
        void Start()
        {
            try
            {
                // Stop background music when the player starts
                GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }

            // Ensure the GameObject has a CharacterController component
            characterController = GetComponent<CharacterController>();

            // Lock and hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Initialize current footstep sounds to wood sounds by default
            currentFootStepSounds = woodFootStepSoundsArr;
        }

        // Update is called once per frame
        void Update()
        {
            // Handle ESC key press to load the "Lobby" scene and unlock cursor
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Lobby");
                Cursor.visible = true; // Make cursor visible
                Cursor.lockState = CursorLockMode.None; // Unlock cursor
                Destroy(PlayerController.Instance.gameObject); // Destroy PlayerController instance
            }

            // Update score UI with the number of correctly classified images
            score.text = "Number of images classified correctly: " + numOfCorrectClassifiedImgs;

            // Movement and rotation logic
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            float curSpeedX = canMove ? (isRunning ? runingSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runingSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            // Jumping logic (not currently used)
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpingPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            // Apply gravity
            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravityForce * Time.deltaTime;
            }

            // Move the player
            characterController.Move(moveDirection * Time.deltaTime);

            // Camera rotation logic
            if (canMove)
            {
                rotationX -= Input.GetAxis("Mouse Y") * lookingSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookInXLimit, lookInXLimit);

                rotationY += Input.GetAxis("Mouse X") * lookingSpeed;

                Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
                Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

                playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, targetRotationX, Time.deltaTime * cameraRotationSmoothness);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationY, Time.deltaTime * cameraRotationSmoothness);
            }

            // Zooming logic
            if (Input.GetButtonDown("Fire2"))
            {
                isZoomed = true;
            }

            if (Input.GetButtonUp("Fire2"))
            {
                isZoomed = false;
            }

            // Smoothly adjust camera FOV based on zoom state
            if (isZoomed)
            {
                playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFieldOfView, Time.deltaTime * cameraZoomSmoothness);
            }
            else
            {
                playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initialFieldOfView, Time.deltaTime * cameraZoomSmoothness);
            }

            // Play footstep sounds based on movement
            if ((curSpeedX != 0f || curSpeedY != 0f) && !isWalking && !isFootstepCoroutineRunning)
            {
                isWalking = true;
                StartCoroutine(PlayFootstepSounds(1.3f / (isRunning ? runingSpeed : walkingSpeed)));
            }
            else if (curSpeedX == 0f && curSpeedY == 0f)
            {
                isWalking = false;
            }
        }

        // Coroutine to play footstep sounds with a delay
        IEnumerator PlayFootstepSounds(float footstepDelay)
        {
            isFootstepCoroutineRunning = true;

            while (isWalking)
            {
                if (currentFootStepSounds.Length > 0)
                {
                    int randomIndex = Random.Range(0, currentFootStepSounds.Length);
                    audioSource.transform.position = footStepAudioPosition.position;
                    audioSource.clip = currentFootStepSounds[randomIndex];
                    audioSource.Play();
                    yield return new WaitForSeconds(footstepDelay);
                }
                else
                {
                    yield break;
                }
            }

            isFootstepCoroutineRunning = false;
        }

        // OnTriggerEnter is called when the Collider other enters the trigger
        private void OnTriggerEnter(Collider other)
        {
            // Detect ground surface and set the current footstep sounds array accordingly
            if (other.CompareTag("Wood"))
            {
                currentFootStepSounds = woodFootStepSoundsArr;
            }
            else if (other.CompareTag("Tile"))
            {
                currentFootStepSounds = tileFootStepSoundsArr;
            }
            else if (other.CompareTag("Carpet"))
            {
                currentFootStepSounds = carpetFootStepSoundsArr;
            }
        }
    }
}
