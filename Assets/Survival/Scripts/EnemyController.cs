/*
The EnemyController script controls the behavior of an enemy character in a game. The enemy can idle, walk between waypoints, 
and chase the player if detected. It uses a NavMeshAgent for navigation and an Animator for animation transitions. 
The script also plays different sounds based on the enemy's state.
*/
using UnityEngine;
using UnityEngine.AI;

namespace Survival
{
    // This class controls the behavior of an enemy character, including patrolling, idling, and chasing the player.
    public class EnemyController : MonoBehaviour
    {
        // Public variables
        public Transform[] waypoints; // Waypoints for the enemy to patrol between.
        public float idleTime = 2f; // Time the enemy idles at each waypoint.
        public float walkSpeed = 2f; // Walking speed.
        public float chaseSpeed = 4f; // Chasing speed.
        public float sightDistance = 10f; // Distance at which the enemy can see the player.
        public AudioClip idleSound; // Sound played when idling.
        public AudioClip walkingSound; // Sound played when walking.
        public AudioClip chasingSound; // Sound played when chasing.

        // Private variables
        private int currentWaypointIndex = 0; // Index of the current waypoint.
        private NavMeshAgent agent; // Reference to the NavMeshAgent component.
        private Animator animator; // Reference to the Animator component.
        private float idleTimer = 0f; // Timer for idling.
        private Transform player; // Reference to the player's transform.
        private AudioSource audioSource; // Reference to the AudioSource component.

        // Enum to represent the enemy's states.
        private enum EnemyState { Idle, Walk, Chase }
        private EnemyState currentState = EnemyState.Idle; // Current state of the enemy.

        private bool isChasingAnimation = false; // Flag to check if the chasing animation is playing.

        // Start is called before the first frame update
        private void Start()
        {
            // Get the necessary components and initialize
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            audioSource = GetComponent<AudioSource>();
            SetDestinationToWaypoint(); // Start the enemy walking to the first waypoint.
        }

        // Update is called once per frame
        private void Update()
        {
            // Handle the enemy's behavior based on its current state
            switch (currentState)
            {
                case EnemyState.Idle:
                    idleTimer += Time.deltaTime;
                    animator.SetBool("IsWalking", false);
                    animator.SetBool("IsChasing", false); // Ensure IsChasing is set to false in the idle state.
                    PlaySound(idleSound);

                    // Switch to walking after idling for a set time
                    if (idleTimer >= idleTime)
                    {
                        NextWaypoint();
                    }

                    CheckForPlayerDetection(); // Check if the player is in sight
                    break;

                case EnemyState.Walk:
                    idleTimer = 0f;
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsChasing", false); // Set IsChasing to false when walking.
                    PlaySound(walkingSound);

                    // Switch to idling when the enemy reaches the waypoint
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        currentState = EnemyState.Idle;
                    }

                    CheckForPlayerDetection(); // Check if the player is in sight
                    break;

                case EnemyState.Chase:
                    idleTimer = 0f;
                    agent.speed = chaseSpeed; // Set the chase speed.
                    agent.SetDestination(player.position); // Chase the player
                    isChasingAnimation = true; // Set to true in chase state.
                    animator.SetBool("IsChasing", true); // Set IsChasing to true in chase state.
                    PlaySound(chasingSound);

                    // Check if the player is out of sight and go back to the walk state.
                    if (Vector3.Distance(transform.position, player.position) > sightDistance)
                    {
                        currentState = EnemyState.Walk;
                        agent.speed = walkSpeed; // Restore walking speed.
                    }
                    break;
            }
        }

        // Method to check if the player is in sight
        private void CheckForPlayerDetection()
        {
            RaycastHit hit;
            Vector3 playerDirection = player.position - transform.position;

            // Perform a raycast to detect the player
            if (Physics.Raycast(transform.position, playerDirection.normalized, out hit, sightDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    currentState = EnemyState.Chase; // Switch to chase state if the player is detected
                    Debug.Log("Player detected!");
                }
            }
        }

        // Method to play the appropriate sound based on the enemy's state
        private void PlaySound(AudioClip soundClip)
        {
            // Play the sound if it's not already playing or if it's a different clip
            if (!audioSource.isPlaying || audioSource.clip != soundClip)
            {
                audioSource.clip = soundClip;
                audioSource.Play();
            }
        }

        // Method to move to the next waypoint
        private void NextWaypoint()
        {
            // Increment the waypoint index and set the destination to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            SetDestinationToWaypoint();
        }

        // Method to set the destination to the current waypoint
        private void SetDestinationToWaypoint()
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            currentState = EnemyState.Walk; // Switch to walk state
            agent.speed = walkSpeed; // Set the walking speed.
            animator.enabled = true;
        }

        // Draw a green raycast line at all times and switch to red when the player is detected.
        private void OnDrawGizmos()
        {
            if (player != null && transform != null)
            {
                Gizmos.color = currentState == EnemyState.Chase ? Color.red : Color.green;
                Gizmos.DrawLine(transform.position, player.position);
            }
            else
            {
                Debug.Log("Player or transform is null");
            }
        }
    }
}
