using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Survival
{
    // This Will Auto Add Character Controller To Gameobject If It's Not Already Applied:
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // // Public Variables

        // Camera:
        public Camera playerCam;

        // Movement Settings:
        public float walkSpeed = 3f;
        public float runSpeed = 10f;
        public float jumpPower = 5f; //edit it if you want to add jumping to the game
        public float gravity = 10f;

        // Camera Settings:
        public float lookSpeed = 2f;
        public float lookXLimit = 75f;
        public float cameraRotationSmooth = 5f;

        // Ground Sounds:
        public AudioClip[] woodFootstepSounds;
        public AudioClip[] tileFootstepSounds;
        public AudioClip[] carpetFootstepSounds;
        public Transform footstepAudioPosition;
        public AudioSource audioSource;
        // score
        public Text score;

        Vector3 moveDirection = Vector3.zero;
        float rotationX = 0;
        float rotationY = 0;

        // Camera Zoom Settings:
        public int ZoomFOV = 35;
        public int initialFOV;
        public float cameraZoomSmooth = 1;

        public int numOfCorrectClassifiedImgs = 0;
        // // Private Variables
        private bool isWalking = false;
        private bool isFootstepCoroutineRunning = false;
        private AudioClip[] currentFootstepSounds;


        private bool isZoomed = false;

        // Can The Player Move?:
        private bool canMove = true;

        CharacterController characterController;

        // The static instance of the GameManager
        private static PlayerController instance;

        // //Initialization
        // Property to access the singleton instance
        public static PlayerController Instance
        {
            get
            {
                if (instance == null)
                {
                    // Find the instance in the scene or create a new one
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
        void Awake()
        {
            // Ensure the instance is set and persists between scenes
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
        void Start()
        {
            try
            {
                GameObject.FindGameObjectWithTag("music").GetComponent<MusicControl>().StopMusic();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            // Ensure We Are Using The Character Controller Component:
            characterController = GetComponent<CharacterController>();

            // Lock And Hide Cursor:
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Initialize current footstep sounds to wood sounds by default
            currentFootstepSounds = woodFootstepSounds;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Load the specified scene when ESC is pressed
                // Loads the "Lobby" scene, which is likely the main menu or exit scene
                SceneManager.LoadScene("Lobby");
                // Make the cursor visible and unlock it
                Cursor.visible = true; // Sets the cursor's visibility to `true`, ensuring that the cursor is visible to the player. This is particularly important in games where the cursor might be hidden by default (e.g., first-person shooters).
                Cursor.lockState = CursorLockMode.None; // Sets the cursor's lock state to `None`, allowing the cursor to move freely around the screen. The `CursorLockMode` enumeration has three possible values:
                // Destroys the PlayerController instance to prevent duplication between scenes
                Destroy(PlayerController.Instance.gameObject);
            }
            score.text = "Number of images classified correctly: " + numOfCorrectClassifiedImgs;

            // Walking / Running In Action:
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            // Jumping In Action:
            if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            {
                moveDirection.y = jumpPower;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);

            // Camera Movement In Action:
            if (canMove)
            {
                rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

                rotationY += Input.GetAxis("Mouse X") * lookSpeed;

                Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
                Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

                playerCam.transform.localRotation = Quaternion.Slerp(playerCam.transform.localRotation, targetRotationX, Time.deltaTime * cameraRotationSmooth);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationY, Time.deltaTime * cameraRotationSmooth);
            }

            // Zooming In Action:
            if (Input.GetButtonDown("Fire2"))
            {
                isZoomed = true;
            }

            if (Input.GetButtonUp("Fire2"))
            {
                isZoomed = false;
            }

            if (isZoomed)
            {
                playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.fieldOfView, ZoomFOV, Time.deltaTime * cameraZoomSmooth);
            }
            else
            {
                playerCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCam.fieldOfView, initialFOV, Time.deltaTime * cameraZoomSmooth);
            }

            // Play footstep sounds when walking
            if ((curSpeedX != 0f || curSpeedY != 0f) && !isWalking && !isFootstepCoroutineRunning)
            {
                isWalking = true;
                StartCoroutine(PlayFootstepSounds(1.3f / (isRunning ? runSpeed : walkSpeed)));
            }
            else if (curSpeedX == 0f && curSpeedY == 0f)
            {
                isWalking = false;
            }
        }

        // Play footstep sounds with a delay based on movement speed
        IEnumerator PlayFootstepSounds(float footstepDelay)
        {
            isFootstepCoroutineRunning = true;

            while (isWalking)
            {
                if (currentFootstepSounds.Length > 0)
                {
                    int randomIndex = Random.Range(0, currentFootstepSounds.Length);
                    audioSource.transform.position = footstepAudioPosition.position;
                    audioSource.clip = currentFootstepSounds[randomIndex];
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

        // Detect ground surface and set the current footstep sounds array accordingly
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Wood"))
            {
                Debug.Log("Wood");

                currentFootstepSounds = woodFootstepSounds;
            }
            else if (other.CompareTag("Tile"))
            {
                Debug.Log("Tile");

                currentFootstepSounds = tileFootstepSounds;
            }
            else if (other.CompareTag("Carpet"))
            {
                Debug.Log("Carpet");

                currentFootstepSounds = carpetFootstepSounds;
            }
        }
    }

}