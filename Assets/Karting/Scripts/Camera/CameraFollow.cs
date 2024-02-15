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
        // Start is called before the first frame update
        void Start()
        {
            playerRB = player.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            Vector3 playerForward = (playerRB.velocity + player.transform.forward).normalized;
            transform.position = Vector3.Lerp(transform.position,
                player.position + player.transform.TransformVector(moveOffset)
                + playerForward * (-5f),
                moveSmoothness * Time.deltaTime);
            // Vector3
            transform.LookAt(player.position + rotOffset);
            // Handle the camera's rotation
            // Quaternion toRotation = Quaternion.LookRotation(player.position - transform.position + rotOffset, player.up);
            // transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, rotSmoothness);
        }

    }
}
