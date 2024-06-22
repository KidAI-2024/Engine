using System;
using System.Collections.Generic;
using UnityEngine;
namespace Karting.Car
{
    public class CarController3 : MonoBehaviour
    {
        private float Gforce;
        private float lastVelocity;
        public Rigidbody Rigidbody { get; private set; }
        public InputData Input { get; private set; }
        public float AirPercent { get; private set; }
        public float GroundPercent { get; private set; }

        public CarController3.Stats baseStats = new CarController3.Stats
        {
            TopSpeed = 10f,
            Acceleration = 5f,
            AccelerationCurve = 4f,
            Braking = 10f,
            ReverseAcceleration = 5f,
            ReverseSpeed = 5f,
            Steer = 5f,
            CoastingDrag = 4f,
            Grip = .95f,
            AddedGravity = 1f,
        };

        [Header("Vehicle Visual")]
        public WheelMeshes wheelMeshes;
        public BrakeLights brakeLights;
        [Header("Physical Wheels")]
        [Tooltip("The physical representations of the Kart's wheels.")]
        public WheelColliders wheelColliders;

        [Header("Vehicle Physics")]
        [Tooltip("The transform that determines the position of the kart's mass.")]
        public Transform CenterOfMass;

        [Range(0.0f, 20.0f), Tooltip("Coefficient used to reorient the kart in the air. The higher the number, the faster the kart will readjust itself along the horizontal plane.")]
        public float AirborneReorientationCoefficient = 3.0f;

        [Header("Drifting")]
        [Range(0.01f, 1.0f), Tooltip("The grip value when drifting.")]
        public float DriftGrip = 0.85f;
        [Range(0.0f, 10.0f), Tooltip("Additional steer when the kart is drifting.")]
        public float DriftAdditionalSteer = 5.0f;
        [Range(1.0f, 30.0f), Tooltip("The higher the angle, the easier it is to regain full grip.")]
        public float MinAngleToFinishDrift = 29.0f;
        [Range(0.01f, 0.99f), Tooltip("Mininum speed percentage to switch back to full grip.")]
        public float MinSpeedPercentToFinishDrift = 0.95f;
        [Range(1.0f, 20.0f), Tooltip("The higher the value, the easier it is to control the drift steering.")]
        public float DriftControl = 16.0f;
        [Range(0.0f, 20.0f), Tooltip("The lower the value, the longer the drift will last without trying to control it by steering.")]
        public float DriftDampening = 8.0f;

        [Header("Suspensions")]
        [Tooltip("The maximum extension possible between the kart's body and the wheels.")]
        [Range(0.0f, 1.0f)]
        public float SuspensionHeight = 0.1f;
        [Range(10.0f, 100000.0f), Tooltip("The higher the value, the stiffer the suspension will be.")]
        public float SuspensionSpring = 70000.0f;
        [Range(0.0f, 5000.0f), Tooltip("The higher the value, the faster the kart will stabilize itself.")]
        public float SuspensionDamp = 5000.0f;
        [Tooltip("Vertical offset to adjust the position of the wheels relative to the kart's body.")]
        [Range(-1.0f, 1.0f)]
        public float WheelsPositionVerticalOffset = 0.0f;

        [Tooltip("Which layers the wheels will detect.")]
        public LayerMask GroundLayers = Physics.DefaultRaycastLayers;

        [Header("Animation")]
        [Tooltip("The damping for the appearance of steering compared to the input.  The higher the number the less damping.")]
        public float steeringAnimationDamping = 10f;
        [Tooltip("The maximum angle in degrees that the front wheels can be turned away from their default positions, when the Steering input is either 1 or -1.")]
        public float maxSteeringAngle = 30f;
        float m_SmoothedSteeringInput;
        const float k_NullInput = 0.01f;
        const float k_NullSpeed = 0.01f;
        Vector3 m_VerticalReference = Vector3.up;

        // Drift params
        public bool WantsToDrift { get; private set; } = false;
        public bool IsDrifting { get; private set; } = false;
        float m_CurrentGrip = 1.0f;
        float m_DriftTurningPower = 0.0f;
        float m_PreviousGroundPercent = 1.0f;

        // can the kart move?
        bool m_CanMove = true;
        List<StatPowerup> m_ActivePowerupList = new List<StatPowerup>();
        CarController3.Stats m_FinalStats;

        Quaternion m_LastValidRotation;
        Vector3 m_LastValidPosition;
        Vector3 m_LastCollisionNormal;
        bool m_HasCollision;
        bool m_InAir = false;

        // used by checkpoints to add powerups to the kart
        public void AddPowerup(StatPowerup statPowerup) => m_ActivePowerupList.Add(statPowerup);
        public void SetCanMove(bool move) => m_CanMove = move;
        public float GetMaxSpeed() => Mathf.Max(m_FinalStats.TopSpeed, m_FinalStats.ReverseSpeed);

        private GameObject webcamFeedController;
        private ProjectController projectController;

        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            UpdateAllSuspensionParams();
            m_CurrentGrip = baseStats.Grip;
            if (brakeLights.LeftLight != null && brakeLights.RightLight != null)
            {
                brakeLights.LeftLight.SetActive(false);
                brakeLights.RightLight.SetActive(false);
            }
            webcamFeedController = GameObject.Find("WebcamFeedController");
            projectController = ProjectController.Instance;
        }

        void FixedUpdate()
        {
            float currVelocity = Rigidbody.velocity.magnitude;
            Gforce = (currVelocity - lastVelocity) / (Time.fixedDeltaTime * Physics.gravity.magnitude);
            lastVelocity = currVelocity;

            UpdateAllSuspensionParams();

            GatherInputs();

            // apply powerups
            TickPowerups();

            // apply our physics properties
            Rigidbody.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

            int groundedCount = CountGroundedWheels();
            // calculate how grounded and airborne we are
            GroundPercent = (float)groundedCount / 4.0f;
            AirPercent = 1 - GroundPercent;

            // apply vehicle physics
            if (m_CanMove)
            {
                MoveVehicle(Input.Accelerate, Input.Brake, Input.TurnInput);
            }
            GroundAirbourne();

            m_PreviousGroundPercent = GroundPercent;

            // UpdateDriftVFXOrientation();
            // Set animation params
            AnimateWheels();
        }
        void LateUpdate()
        {
            ApplyWheelPositions();
        }

        private void AnimateWheels()
        {
            m_SmoothedSteeringInput = Mathf.MoveTowards(m_SmoothedSteeringInput, Input.TurnInput,
              steeringAnimationDamping * Time.deltaTime);

            // Steer front wheels
            float rotationAngle = m_SmoothedSteeringInput * maxSteeringAngle;

            wheelColliders.FrontLeftWheel.steerAngle = rotationAngle;
            wheelColliders.FrontRightWheel.steerAngle = rotationAngle;
        }

        int CountGroundedWheels()
        {
            int groundedCount = 0;
            if (wheelColliders.FrontLeftWheel.isGrounded && wheelColliders.FrontLeftWheel.GetGroundHit(out WheelHit hit))
                groundedCount++;
            if (wheelColliders.FrontRightWheel.isGrounded && wheelColliders.FrontRightWheel.GetGroundHit(out hit))
                groundedCount++;
            if (wheelColliders.RearLeftWheel.isGrounded && wheelColliders.RearLeftWheel.GetGroundHit(out hit))
                groundedCount++;
            if (wheelColliders.RearRightWheel.isGrounded && wheelColliders.RearRightWheel.GetGroundHit(out hit))
                groundedCount++;
            return groundedCount;
        }
        void UpdateSuspensionParams(WheelCollider wheel)
        {
            wheel.suspensionDistance = SuspensionHeight;
            wheel.center = new Vector3(0.0f, WheelsPositionVerticalOffset, 0.0f);
            JointSpring spring = wheel.suspensionSpring;
            spring.spring = SuspensionSpring;
            spring.damper = SuspensionDamp;
            wheel.suspensionSpring = spring;
        }
        void UpdateAllSuspensionParams()
        {
            UpdateSuspensionParams(wheelColliders.FrontLeftWheel);
            UpdateSuspensionParams(wheelColliders.FrontRightWheel);
            UpdateSuspensionParams(wheelColliders.RearLeftWheel);
            UpdateSuspensionParams(wheelColliders.RearRightWheel);
        }

        public virtual float GetGforce()
        {
            return Gforce;
        }
        void ApplyWheelPositions()
        {
            UpdateWheelPosition(wheelColliders.FrontLeftWheel, wheelMeshes.FrontLeftWheel);
            UpdateWheelPosition(wheelColliders.FrontRightWheel, wheelMeshes.FrontRightWheel);
            UpdateWheelPosition(wheelColliders.RearLeftWheel, wheelMeshes.RearLeftWheel);
            UpdateWheelPosition(wheelColliders.RearRightWheel, wheelMeshes.RearRightWheel);
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
        public void Reset()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }

        public float LocalSpeed()
        {
            if (m_CanMove)
            {
                float dot = Vector3.Dot(transform.forward, Rigidbody.velocity);
                if (Mathf.Abs(dot) > 0.1f)
                {
                    float speed = Rigidbody.velocity.magnitude;
                    return dot < 0 ? -(speed / m_FinalStats.ReverseSpeed) : (speed / m_FinalStats.TopSpeed);
                }
                return 0f;
            }
            else
            {
                // use this value to play kart sound when it is waiting the race start countdown.
                return Input.Accelerate ? 1.0f : 0.0f;
            }
        }
        void MoveVehicle(bool accelerate, bool brake, float turnInput)
        {
            float accelInput = (accelerate ? 1.0f : 0.0f) - (brake ? 1.0f : 0.0f);

            Vector3 localVel = transform.InverseTransformVector(Rigidbody.velocity);

            bool accelDirectionIsFwd = accelInput >= 0;
            bool localVelDirectionIsFwd = localVel.z >= 0;

            // use the max speed for the direction we are going--forward or reverse.
            float maxSpeed = localVelDirectionIsFwd ? m_FinalStats.TopSpeed : m_FinalStats.ReverseSpeed;
            float currentSpeed = Rigidbody.velocity.magnitude;
            bool isBraking = (localVelDirectionIsFwd && brake) || (!localVelDirectionIsFwd && accelerate);
            // apply inputs to forward/backward
            float turningPower = IsDrifting ? m_DriftTurningPower : turnInput * m_FinalStats.Steer;
            // manage break lights
            ManageBreakLights(isBraking);

            // forward movement
            ForwardMovement(accelInput, currentSpeed, maxSpeed, isBraking, turningPower, accelDirectionIsFwd);

            if (GroundPercent > 0.0f)
            {
                if (m_InAir)
                {
                    m_InAir = false;
                }
                // Drift
                // Update angular velocity
                UpdateAngularVelocity(turningPower, localVelDirectionIsFwd, accelDirectionIsFwd);
                // Drift Management
                StartDrifting(isBraking, currentSpeed, maxSpeed, accelInput, turningPower);
                DriftManagement(isBraking, turnInput, currentSpeed, maxSpeed, accelInput, turningPower, localVel);
            }
            else
            {
                m_InAir = true;
            }

            UpdateVerticalReference();
            // Airborne / Half on ground management
            AirborneManagement();
        }

        private void ManageBreakLights(bool isBraking)
        {
            if (brakeLights.LeftLight != null && brakeLights.RightLight != null)
            {
                brakeLights.LeftLight.SetActive(isBraking);
                brakeLights.RightLight.SetActive(isBraking);
            }
        }

        private void ForwardMovement(float accelInput, float currentSpeed, float maxSpeed, bool isBraking, float turningPower, bool accelDirectionIsFwd)
        {
            // manual acceleration curve coefficient scalar
            float accelerationCurveCoeff = 5;
            // get the acceleration for the direction we are going
            float accelPower = accelDirectionIsFwd ? m_FinalStats.Acceleration : m_FinalStats.ReverseAcceleration;
            // calculate the acceleration ramp based on the current speed and maximum speed
            float accelRampT = currentSpeed / maxSpeed;
            float multipliedAccelerationCurve = m_FinalStats.AccelerationCurve * accelerationCurveCoeff;
            float accelRamp = Mathf.Lerp(multipliedAccelerationCurve, 1, accelRampT * accelRampT);
            // if we are braking (moving reverse to where we are going)
            // use the braking accleration instead
            float finalAccelPower = isBraking ? m_FinalStats.Braking : accelPower;

            // calculate the final acceleration by multiplying the acceleration power with the acceleration ramp
            float finalAcceleration = finalAccelPower * accelRamp;

            // calculate the turning angle based on the turning power and the car's transform
            Quaternion turnAngle = Quaternion.AngleAxis(turningPower, transform.up);

            // calculate the forward direction based on the turning angle and the car's forward vector
            Vector3 fwd = turnAngle * transform.forward;
            // calculate the movement vector by combining the forward direction, acceleration input, final acceleration,
            // and a multiplier based on whether the car has collision or is on the ground
            Vector3 movement = fwd * accelInput * finalAcceleration * ((m_HasCollision || GroundPercent > 0.0f) ? 1.0f : 0.0f);
            // FrontRightWheel.steerAngle = turnAngle.eulerAngles.y;
            // FrontLeftWheel.steerAngle = turnAngle.eulerAngles.y;
            // check if the current speed is over the maximum speed and the car is not braking
            bool wasOverMaxSpeed = currentSpeed >= maxSpeed;

            // if over max speed, cannot accelerate faster.
            if (wasOverMaxSpeed && !isBraking)
                movement *= 0.0f;

            // calculate the new velocity by adding the movement vector to the current velocity
            Vector3 newVelocity = Rigidbody.velocity + movement * Time.fixedDeltaTime;
            newVelocity.y = Rigidbody.velocity.y;

            // clamp the max speed if the car is on the ground and not already over the max speed
            if (GroundPercent > 0.0f && !wasOverMaxSpeed)
            {
                newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed);
            }

            // if the acceleration input is close to zero and the car is on the ground, apply coasting drag
            if (Mathf.Abs(accelInput) < k_NullInput && GroundPercent > 0.0f)
            {
                newVelocity = Vector3.MoveTowards(newVelocity, new Vector3(0, Rigidbody.velocity.y, 0), Time.fixedDeltaTime * m_FinalStats.CoastingDrag);
            }
            // update the car's velocity with the new velocity
            Rigidbody.velocity = newVelocity;
        }

        private void AirborneManagement()
        {
            bool validPosition = false;
            validPosition = GroundPercent > 0.7f && !m_HasCollision && Vector3.Dot(m_VerticalReference, Vector3.up) > 0.9f;
            if (GroundPercent < 0.7f)
            {
                Rigidbody.angularVelocity = new Vector3(0.0f, Rigidbody.angularVelocity.y * 0.98f, 0.0f);
                Vector3 finalOrientationDirection = Vector3.ProjectOnPlane(transform.forward, m_VerticalReference);
                finalOrientationDirection.Normalize();
                if (finalOrientationDirection.sqrMagnitude > 0.0f)
                {
                    Rigidbody.MoveRotation(Quaternion.Lerp(Rigidbody.rotation, Quaternion.LookRotation(finalOrientationDirection, m_VerticalReference), Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime)));
                }
            }
            else if (validPosition)
            {
                m_LastValidPosition = transform.position;
                m_LastValidRotation.eulerAngles = new Vector3(0.0f, transform.rotation.y, 0.0f);
            }
        }

        private void UpdateVerticalReference()
        {
            if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out RaycastHit hit, 3.0f, 1 << 9 | 1 << 10 | 1 << 11)) // Layer: ground (9) / Environment(10) / Track (11)
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > hit.normal.y) ? m_LastCollisionNormal : hit.normal;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime * (GroundPercent > 0.0f ? 10.0f : 1.0f)));    // Blend faster if on ground
            }
            else
            {
                Vector3 lerpVector = (m_HasCollision && m_LastCollisionNormal.y > 0.0f) ? m_LastCollisionNormal : Vector3.up;
                m_VerticalReference = Vector3.Slerp(m_VerticalReference, lerpVector, Mathf.Clamp01(AirborneReorientationCoefficient * Time.fixedDeltaTime));
            }
        }

        private void StartDrifting(bool isBraking, float currentSpeed, float maxSpeed, float accelInput, float turningPower)
        {
            // If the karts lands with a forward not in the velocity direction, we start the drift
            if (GroundPercent >= 0.0f && m_PreviousGroundPercent < 0.1f)
            {
                Vector3 flattenVelocity = Vector3.ProjectOnPlane(Rigidbody.velocity, m_VerticalReference).normalized;
                if (Vector3.Dot(flattenVelocity, transform.forward * Mathf.Sign(accelInput)) < Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad))
                {
                    IsDrifting = true;
                    m_CurrentGrip = DriftGrip;
                    m_DriftTurningPower = 0.0f;
                }
            }
            if (!IsDrifting)
            {
                if ((WantsToDrift || isBraking) && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
                {
                    IsDrifting = true;
                    m_DriftTurningPower = turningPower + (Mathf.Sign(turningPower) * DriftAdditionalSteer);
                    m_CurrentGrip = DriftGrip;
                }
            }
        }

        private void DriftManagement(bool isBraking, float turnInput, float currentSpeed, float maxSpeed, float accelInput, float turningPower, Vector3 localVel)
        {
            if (IsDrifting)
            {
                float turnInputAbs = Mathf.Abs(turnInput);
                if (turnInputAbs < k_NullInput)
                    m_DriftTurningPower = Mathf.MoveTowards(m_DriftTurningPower, 0.0f, Mathf.Clamp01(DriftDampening * Time.fixedDeltaTime));

                // Update the turning power based on input
                float driftMaxSteerValue = m_FinalStats.Steer + DriftAdditionalSteer;
                m_DriftTurningPower = Mathf.Clamp(m_DriftTurningPower + (turnInput * Mathf.Clamp01(DriftControl * Time.fixedDeltaTime)), -driftMaxSteerValue, driftMaxSteerValue);

                bool facingVelocity = Vector3.Dot(Rigidbody.velocity.normalized, transform.forward * Mathf.Sign(accelInput)) > Mathf.Cos(MinAngleToFinishDrift * Mathf.Deg2Rad);

                bool canEndDrift = true;
                if (isBraking)
                    canEndDrift = false;
                else if (!facingVelocity)
                    canEndDrift = false;
                else if (turnInputAbs >= k_NullInput && currentSpeed > maxSpeed * MinSpeedPercentToFinishDrift)
                    canEndDrift = false;

                if (canEndDrift || currentSpeed < k_NullSpeed)
                {
                    // No Input, and car aligned with speed direction => Stop the drift
                    IsDrifting = false;
                    m_CurrentGrip = m_FinalStats.Grip;
                }

            }
            // rotate rigidbody's velocity as well to generate immediate velocity redirection
            // manual velocity steering coefficient
            float velocitySteering = 25f;

            // rotate our velocity based on current steer value
            Rigidbody.velocity = Quaternion.AngleAxis(turningPower * Mathf.Sign(localVel.z) * velocitySteering * m_CurrentGrip * Time.fixedDeltaTime, transform.up) * Rigidbody.velocity;

        }

        private void UpdateAngularVelocity(float turningPower, bool localVelDirectionIsFwd, bool accelDirectionIsFwd)
        {
            // manual angular velocity coefficient
            float angularVelocitySteering = 0.4f;
            float angularVelocitySmoothSpeed = 20f;

            // turning is reversed if we're going in reverse and pressing reverse
            if (!localVelDirectionIsFwd && !accelDirectionIsFwd)
                angularVelocitySteering *= -1.0f;

            var angularVel = Rigidbody.angularVelocity;

            // move the Y angular velocity towards our target
            angularVel.y = Mathf.MoveTowards(angularVel.y, turningPower * angularVelocitySteering, Time.fixedDeltaTime * angularVelocitySmoothSpeed);

            // apply the angular velocity
            Rigidbody.angularVelocity = angularVel;
        }

        void GatherInputs()
        {
            // reset input
            Input = new InputData();
            WantsToDrift = false;

            // gather nonzero input from our sources
            Input = GenerateInput();
            WantsToDrift = Input.Brake && Vector3.Dot(Rigidbody.velocity, transform.forward) > 0.0f;
        }
        InputData GenerateInput()
        {
            InputData input = new InputData();
            input.Accelerate = UnityEngine.Input.GetAxis("Vertical") > 0.0f;
            input.Brake = UnityEngine.Input.GetAxis("Vertical") < 0.0f || UnityEngine.Input.GetKey(KeyCode.Space);
            input.TurnInput = UnityEngine.Input.GetAxis("Horizontal");
            if (webcamFeedController == null)
            {
                return input;
            }
            Karting.WebcamFeed.KartingWebcamFeedController webcamControllerObj = webcamFeedController.GetComponent<Karting.WebcamFeed.KartingWebcamFeedController>();
            if (webcamControllerObj != null)
            {
                int pythonPredictedClass = webcamControllerObj.pythonPredictedClass;
                if (pythonPredictedClass != -1)
                {
                    string predictedClass = webcamControllerObj.UnityPredictedClass;
                    string predictedControl = projectController.classesToControlsMap[predictedClass];
                    input.Accelerate = input.Accelerate || predictedControl == "Forward";
                    input.Brake = input.Brake || predictedControl == "Backward";
                    input.TurnInput = input.TurnInput + (predictedControl == "Left" ? -1 : 0) + (predictedControl == "Right" ? 1 : 0);
                }
            }
            return input;
        }

        void TickPowerups()
        {
            // remove all powerups that have reached their max time
            m_ActivePowerupList.RemoveAll((p) => { return p.ElapsedTime > p.MaxTime; });

            // empty stats to sum all powerups
            var powerups = new Stats();

            // add up all our powerups
            for (int i = 0; i < m_ActivePowerupList.Count; i++)
            {
                var p = m_ActivePowerupList[i];

                // increase elapsed time
                p.ElapsedTime += Time.fixedDeltaTime;

                // add powerups
                powerups += p.modifiers;
                // Debug.Log("Powerup: " + m_ActivePowerupList[i].PowerUpID + " Elapsed Time: " + m_ActivePowerupList[i].ElapsedTime + " Max Time: " + p.MaxTime);
            }

            // add powerups to our final stats
            m_FinalStats = baseStats + powerups;

            // clamp values in finalstats
            m_FinalStats.Grip = Mathf.Clamp(m_FinalStats.Grip, 0, 1);
        }
        void GroundAirbourne()
        {
            // while in the air, fall faster
            if (AirPercent >= 1)
            {
                Rigidbody.velocity += Physics.gravity * Time.fixedDeltaTime * m_FinalStats.AddedGravity;
            }
        }
        void OnCollisionEnter(Collision collision) => m_HasCollision = true;
        void OnCollisionExit(Collision collision) => m_HasCollision = false;

        void OnCollisionStay(Collision collision)
        {
            m_HasCollision = true;
            m_LastCollisionNormal = Vector3.zero;
            float dot = -1.0f;

            foreach (var contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > dot)
                    m_LastCollisionNormal = contact.normal;
            }
        }
        [System.Serializable]
        public struct WheelColliders
        {
            public WheelCollider FrontLeftWheel;
            public WheelCollider FrontRightWheel;
            public WheelCollider RearLeftWheel;
            public WheelCollider RearRightWheel;
        }
        [System.Serializable]
        public struct WheelMeshes
        {
            public MeshRenderer FrontLeftWheel;
            public MeshRenderer FrontRightWheel;
            public MeshRenderer RearLeftWheel;
            public MeshRenderer RearRightWheel;
        }
        [System.Serializable]
        public struct BrakeLights
        {
            public GameObject LeftLight;
            public GameObject RightLight;
        }
        [System.Serializable]
        public class StatPowerup
        {
            public CarController3.Stats modifiers;
            public string PowerUpID;
            public float ElapsedTime = 0.0f;
            public float MaxTime;
        }

        [System.Serializable]
        public struct Stats
        {
            [Header("Movement Settings")]
            [Min(0.001f), Tooltip("Top speed attainable when moving forward.")]
            public float TopSpeed;

            [Tooltip("How quickly the kart reaches top speed.")]
            public float Acceleration;

            [Min(0.001f), Tooltip("Top speed attainable when moving backward.")]
            public float ReverseSpeed;

            [Tooltip("How quickly the kart reaches top speed, when moving backward.")]
            public float ReverseAcceleration;

            [Tooltip("How quickly the kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
            [Range(0.2f, 1)]
            public float AccelerationCurve;

            [Tooltip("How quickly the kart slows down when the brake is applied.")]
            public float Braking;

            [Tooltip("How quickly the kart will reach a full stop when no inputs are made.")]
            public float CoastingDrag;

            [Range(0.0f, 1.0f)]
            [Tooltip("The amount of side-to-side friction.")]
            public float Grip;

            [Tooltip("How tightly the kart can turn left or right.")]
            public float Steer;

            [Tooltip("Additional gravity for when the kart is in the air.")]
            public float AddedGravity;

            // allow for stat adding for powerups.
            public static Stats operator +(Stats a, Stats b)
            {
                return new Stats
                {
                    Acceleration = a.Acceleration + b.Acceleration,
                    AccelerationCurve = a.AccelerationCurve + b.AccelerationCurve,
                    Braking = a.Braking + b.Braking,
                    CoastingDrag = a.CoastingDrag + b.CoastingDrag,
                    AddedGravity = a.AddedGravity + b.AddedGravity,
                    Grip = a.Grip + b.Grip,
                    ReverseAcceleration = a.ReverseAcceleration + b.ReverseAcceleration,
                    ReverseSpeed = a.ReverseSpeed + b.ReverseSpeed,
                    TopSpeed = a.TopSpeed + b.TopSpeed,
                    Steer = a.Steer + b.Steer,
                };
            }
        }
        public struct InputData
        {
            public bool Accelerate;
            public bool Brake;
            public float TurnInput;
        }
    }

}
