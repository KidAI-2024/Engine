using UnityEngine;

namespace Karting.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Smoothness")]
        public float moveSmoothness;
        public float rotSmoothness;

        [Header("Offsets")]
        public Vector3 moveOffset;
        public Vector3 rotOffset;

        [Header("Target")]
        public Transform carTarget;

        void LateUpdate()
        {
            Move();
            Rotate();
        }

        void Move()
        {
            Vector3 targetPos = carTarget.TransformPoint(moveOffset);
            transform.position = Vector3.Lerp(transform.position, targetPos, moveSmoothness * Time.deltaTime);
        }

        void Rotate()
        {
            var direction = carTarget.position - transform.position;
            var rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
        }
    }
}
