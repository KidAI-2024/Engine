using UnityEngine;
namespace Karting.Car
{
    public class CarController2 : MonoBehaviour
    {
        public WheelColliders colliders;
        public WheelMeshes wheelMeshes;
        private float gasInput;
        private float brakeInput;
        private float steeringInput;
        public float motorPower;
        public float brakePower;
        private float slipAngle;
        private float speed;
        private Rigidbody playerRB;
        public AnimationCurve steeringCurve;
        // Start is called before the first frame update
        void Start()
        {
            playerRB = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            speed = playerRB.velocity.magnitude;
            GetInput();
            Move();
            Brake();
            Steer();
            ApplyWheelPositions();
        }
        void GetInput()
        {
            gasInput = Input.GetAxis("Vertical");
            steeringInput = Input.GetAxis("Horizontal");
            slipAngle = Vector3.Angle(transform.forward, playerRB.velocity - transform.forward);
            float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
            if (movingDirection < -0.5f && gasInput > 0)
            {
                brakeInput = Mathf.Abs(gasInput);
            }
            else if (movingDirection > 0.5f && gasInput < 0)
            {
                brakeInput = Mathf.Abs(gasInput);
            }
            else
            {
                brakeInput = 0;
            }
        }
        void Brake()
        {
            // 70% of the brake power to the front wheels
            colliders.Wheel_FR.brakeTorque = brakeInput * brakePower * 0.7f;
            colliders.Wheel_FL.brakeTorque = brakeInput * brakePower * 0.7f;
            // 30% of the brake power to the rear wheels
            colliders.Wheel_BR.brakeTorque = brakeInput * brakePower * 0.3f;
            colliders.Wheel_BL.brakeTorque = brakeInput * brakePower * 0.3f;
        }
        void Move()
        {
            // Rear wheel drive
            colliders.Wheel_BR.motorTorque = motorPower * gasInput;
            colliders.Wheel_BL.motorTorque = motorPower * gasInput;
            // Front wheel drive
            // colliders.FRWheel.motorTorque = motorPower * gasInput;
            // colliders.FLWheel.motorTorque = motorPower * gasInput;
        }
        void Steer()
        {

            float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
            if (slipAngle < 120f)
            {
                steeringAngle += Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
            }
            steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
            colliders.Wheel_FR.steerAngle = steeringAngle;
            colliders.Wheel_FL.steerAngle = steeringAngle;
        }
        // Update all the wheel positions
        void ApplyWheelPositions()
        {
            UpdateWheelPosition(colliders.Wheel_FR, wheelMeshes.Wheel_FR);
            UpdateWheelPosition(colliders.Wheel_FL, wheelMeshes.Wheel_FL);
            UpdateWheelPosition(colliders.Wheel_BR, wheelMeshes.Wheel_BR);
            UpdateWheelPosition(colliders.Wheel_BL, wheelMeshes.Wheel_BL);
        }
        // Update the wheel position based on the wheel collider
        void UpdateWheelPosition(WheelCollider coll, MeshRenderer wheelMesh)
        {
            Quaternion quat;
            Vector3 position;
            coll.GetWorldPose(out position, out quat);
            // set the wheel position to the position of the wheel collider
            wheelMesh.transform.position = position;
            // set the wheel rotation to the rotation of the wheel collider
            wheelMesh.transform.rotation = quat;
        }
        [System.Serializable]
        public struct WheelColliders
        {
            public WheelCollider Wheel_FR;
            public WheelCollider Wheel_FL;
            public WheelCollider Wheel_BR;
            public WheelCollider Wheel_BL;
        }
        [System.Serializable]
        public struct WheelMeshes
        {
            public MeshRenderer Wheel_FR;
            public MeshRenderer Wheel_FL;
            public MeshRenderer Wheel_BR;
            public MeshRenderer Wheel_BL;
        }
    }
}
