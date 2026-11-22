
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController2 : MonoBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 0f;
    public float turnSpeed = 30f;
    public float force = 1500f;
    public float topSpeed = 30f;
    public float DragSpeed = 0f;
    public float currentSpeed = 0f;
    private float previousSpeed = 0f;

    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Wheel Models")]
    public Transform frontLeftModel;
    public Transform frontRightModel;
    public Transform rearLeftModel;
    public Transform rearRightModel;

    [Header("Braking")]
    public float brakeForce = 3000f;
    public bool isBraking = false;

    [Header("Collision Tracking")]
    public int obstacleHits = 0;

    private Vector2 movement;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = 1200f;
        rb.useGravity = true;

        Debug.Log("My awesome car is ready!");
    }

    void Update()
    {
        UpdateWheelVisuals();
    }

    void FixedUpdate()
    {
        MoveVehicle();
        TurnVehicle();
    }

    void MoveVehicle()
    {
        float verticalInput = movement.x;

        currentSpeed = rb.linearVelocity.magnitude;

        // Check whether driven wheels touch the ground.
        bool rearLeftGrounded = rearLeftWheel.isGrounded;
        bool rearRightGrounded = rearRightWheel.isGrounded;

        float motorTorque = 0f;

        // Acceleration is applied through rear wheels.
        if (!isBraking && currentSpeed < topSpeed)
        {
            motorTorque = verticalInput * force;
        }

        rearLeftWheel.motorTorque = motorTorque;
        rearRightWheel.motorTorque = motorTorque;

        // Front wheels do not drive the car.
        frontLeftWheel.motorTorque = 0f;
        frontRightWheel.motorTorque = 0f;

        // Apply braking.
        float brake = isBraking ? brakeForce : 0f;

        frontLeftWheel.brakeTorque = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque = brake;
        rearRightWheel.brakeTorque = brake;

        // Speed measurements.
        DragSpeed = previousSpeed - currentSpeed;
        previousSpeed = currentSpeed;
        moveSpeed = currentSpeed;

        if (rearLeftGrounded || rearRightGrounded)
        {
            if (verticalInput != 0f)
            {
                Debug.Log("Acceleration activates");
            }
        }
    }

    void TurnVehicle()
    {
        float horizontalInput = movement.y;

        // Steer using the front wheels.
        float steeringAngle = horizontalInput * turnSpeed;

        frontLeftWheel.steerAngle = steeringAngle;
        frontRightWheel.steerAngle = steeringAngle;
    }

    void UpdateWheelVisuals()
    {
        UpdateSingleWheel(frontLeftWheel, frontLeftModel);
        UpdateSingleWheel(frontRightWheel, frontRightModel);
        UpdateSingleWheel(rearLeftWheel, rearLeftModel);
        UpdateSingleWheel(rearRightWheel, rearRightModel);
    }

    void UpdateSingleWheel(
        WheelCollider wheelCollider,
        Transform wheelModel)
    {
        if (wheelCollider == null || wheelModel == null)
            return;

        Vector3 position;
        Quaternion rotation;

        wheelCollider.GetWorldPose(out position, out rotation);

        wheelModel.position = position;
        wheelModel.rotation = rotation;
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log(other.gameObject.name);

        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Collision with obstacle detected");

            obstacleHits++;

            Debug.Log("Obstacles Hit: " + obstacleHits);
            Debug.Log("Collision Time: " + Time.time);

            rb.AddForce(
                -transform.right * 5f,
                ForceMode.VelocityChange
            );
        }
    }

    public void OnBrake(InputValue value)
    {
        isBraking = value.isPressed;
    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }
}
