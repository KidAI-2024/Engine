using UnityEngine;

namespace Karting.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        public Transform player;
        private Rigidbody playerRB;
        [Header("Offsets")]
        public Vector3 moveOffset;
        public Vector3 rotOffset;
        [Header("Smoothness")]
        public float moveSmoothness;
        public float rotSmoothness;
        public float velocityDamping = 0.2f; // Adjust this value for velocity damping

        // Start is called before the first frame update
        void Start()
        {
            playerRB = player.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        // void LateUpdate()
        // {
        //     Vector3 playerForward = (playerRB.velocity + player.transform.forward).normalized;
        //     transform.position = Vector3.Lerp(transform.position,
        //         player.position + player.transform.TransformVector(moveOffset)
        //         + playerForward * (-5f),
        //         moveSmoothness * Time.deltaTime);
        //     // Vector3
        //     transform.LookAt(player.position + rotOffset);
        //     // Handle the camera's rotation
        //     // Quaternion toRotation = Quaternion.LookRotation(player.position - transform.position + rotOffset, player.up);
        //     // transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotSmoothness);
        // }
        void LateUpdate()
        {
            Vector3 playerForward = (playerRB.velocity + player.transform.forward * velocityDamping).normalized; // Dampen velocity
            Vector3 targetPosition = player.position + player.transform.TransformVector(moveOffset) + playerForward * (-5f);
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position + rotOffset);

            // Smoothly move and rotate the camera
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothness * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSmoothness * Time.deltaTime);
        }

    }
}
