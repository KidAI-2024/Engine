using UnityEngine;

namespace Karting.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        public float lerpTime = 3.5f;
        [Range(2, 3.5f)] public float forwardDistance = 3;
        private float accelerationEffect;

        public GameObject atachedVehicle;
        private int locationIndicator = 0;
        private Car.CarController3 controllerRef;

        private Vector3 newPos;
        private Transform target;
        private GameObject focusPoint;

        public float distance = 2;

        public Vector2[] cameraPos;
        void Start()
        {
            cameraPos = new Vector2[4];
            // cameraPos[0] = new Vector2(2, 0);
            cameraPos[0] = new Vector2(8.9f, 1.2f);
            cameraPos[1] = new Vector2(10.73f, 0.73f);

            focusPoint = atachedVehicle.transform.Find("focus").gameObject;

            target = focusPoint.transform;
            controllerRef = atachedVehicle.GetComponent<Car.CarController3>();
        }

        private void FixedUpdate()
        {
            UpdateCam();
        }

        public void CycleCamera()
        {
            if (locationIndicator >= cameraPos.Length - 1 || locationIndicator < 0) locationIndicator = 0;
            else locationIndicator++;
        }
        public void UpdateCam()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                CycleCamera();
            }

            // Calculate the new camera position based on the target's position and camera position offsets
            newPos = target.position - (target.forward * cameraPos[locationIndicator].x) + (target.up * cameraPos[locationIndicator].y);

            // Calculate the acceleration effect based on the vehicle's G-force
            accelerationEffect = Mathf.Lerp(accelerationEffect, controllerRef.GetGforce() * 3.5f, 2 * Time.deltaTime);

            // Move the camera towards the focus point using a lerp function
            transform.position = Vector3.Lerp(transform.position, focusPoint.transform.position, lerpTime * Time.deltaTime);

            // Calculate the distance between the camera and the new position
            distance = Mathf.Pow(Vector3.Distance(transform.position, newPos), forwardDistance);

            // Move the camera towards the new position using a move towards function
            transform.position = Vector3.MoveTowards(transform.position, newPos, distance * Time.deltaTime);

            // Rotate the camera based on the acceleration effect
            transform.GetChild(0).transform.localRotation = Quaternion.Lerp(transform.GetChild(0).transform.localRotation, Quaternion.Euler(-accelerationEffect, 0, 0), 5 * Time.deltaTime);

            // Make the camera look at the target
            transform.LookAt(target.transform);
        }

    }
}
