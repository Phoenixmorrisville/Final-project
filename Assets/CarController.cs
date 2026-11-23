using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    // Speed settings - editable in the Unity Inspector
    public float moveSpeed = 0f;
    public float force = 500f;
    public float topSpeed = 30f;
    public float DragSpeed = 0f;
    public float currentSpeed = 0f;
    private float previousSpeed = 0f;
    public int obstacleHits = 0;
    public float torqueReductionRange = 10f;
    Vector2 movement;
    //Turning
    public float steeringSpeed = 60f;
    public float currentSteeringAngle = 0f;
    public float lowSpeedTurnAngle = 35f;
    public float highSpeedTurnAngle = 8f;
    public float steeringReductionSpeed = 25f;
    
    //Braking
    public float brakeForce = 3000f;
    public float coastBrakeForce = 100f;
    public bool isBraking = false;
    private PlayerInput input;
    public InputActionReference brakeAction;


    //Wheel colliders
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;



    // wheel models
    public Transform frontLeftModel;
    public Transform frontRightModel;
    public Transform rearLeftModel;
    public Transform rearRightModel;
    // Start is called once before the first frame update

    // Rigidbody Inspector properties:
    // Mass
    // Use Gravity
    // Linear Damping


    private Rigidbody rb;
    void Start()
    {
        Debug.Log("My awesome car is ready! Hello from MyCar");

        // GetComponent<Rigidbody>() finds the Rigidbody attached to
        // this GameObject (MyCar) and stores a reference to it in rb.
        rb = GetComponent<Rigidbody>();

        //rb.mass = 1200f;
        rb.useGravity = true;

        //rb.AddForce(Vector3.right * force, ForceMode.Force);
        //rb.AddForce(Vector3.right * force, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWheelVisuals();

        isBraking = brakeAction.action.IsPressed();
    }

    void FixedUpdate()
    {
        MoveVehicle();
        TurnVehicle();
    }

    void MoveVehicle()
    {
        //currentSpeed = rb.linearVelocity.magnitude;
        float verticalInput = movement.y;
        //float brakeInput = 1f;
        float motorTorque = 0f;

        //frontLeftWheel.brakeTorque = 0f;

        currentSpeed = rb.linearVelocity.magnitude * 2.23694f;

        float forwardSpeedMPH = Vector3.Dot(
            rb.linearVelocity,
            transform.forward
        ) * 2.23694f;

        float brake = 0f;

        if (isBraking)
        {
            // Player is pressing the brake.
            brake = brakeForce;
        }
        else if (Mathf.Abs(verticalInput) < 0.1f)
        {
            // Player released the accelerator.
            brake = coastBrakeForce;
        }
        else
        {
            // Player is accelerating.
            brake = 0f;
        }

        frontLeftWheel.brakeTorque = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque = brake;
        rearRightWheel.brakeTorque = brake;

        // Gradually reduce torque near top speed.
        float reductionRange = Mathf.Max(
            torqueReductionRange, 0.01f
        );

        if (!isBraking)
        {
            if (verticalInput > 0f &&
                forwardSpeedMPH < topSpeed)
            {
                float speedRemaining =
                    topSpeed - forwardSpeedMPH;

                float torqueMultiplier = Mathf.Clamp01(
                    speedRemaining / reductionRange
                );

                motorTorque =
                    verticalInput * force * torqueMultiplier;
            }
            else if (verticalInput < 0f &&
                     forwardSpeedMPH > -topSpeed)
            {
                float speedRemaining =
                    topSpeed + forwardSpeedMPH;

                float torqueMultiplier = Mathf.Clamp01(
                    speedRemaining / reductionRange
                );

                motorTorque =
                    verticalInput * force * torqueMultiplier;
            }
        }


        

        rearLeftWheel.motorTorque = motorTorque;
        rearRightWheel.motorTorque = motorTorque;

        // Front wheels do not drive the car.
        frontLeftWheel.motorTorque = 0f;
        frontRightWheel.motorTorque = 0f;
        //rb.linearVelocity = rb.linearVelocity.normalized
        //tried to use addforce for movement but it didnt turn with the car
        if (rb.linearVelocity.magnitude < topSpeed)
        {
            //rb.AddForce(transform.right * verticalInput * moveSpeed, ForceMode.Acceleration);
        }
        //transform.Translate(new Vector3(verticalInput * moveSpeed * Time.deltaTime, 0, 0));

        if (verticalInput != 0)
        {
            Debug.Log("Vehicle is moving.");
        }
        previousSpeed = currentSpeed;
    }

    void TurnVehicle()
    {
        float horizontalInput = movement.x;

        // Get the car's current speed in meters per second.
        float speed = rb.linearVelocity.magnitude;

        // Convert speed into a value between 0 and 1.
        float speedPercent = Mathf.Clamp01(speed / steeringReductionSpeed);

        // Calculate the maximum steering angle based on speed.
        float adjustedTurnAngle = Mathf.Lerp(lowSpeedTurnAngle, highSpeedTurnAngle, speedPercent);

        // Calculate the target steering angle.
        float targetSteeringAngle = horizontalInput * adjustedTurnAngle;

        // Gradually move toward the target angle.
        currentSteeringAngle = Mathf.MoveTowards(currentSteeringAngle, targetSteeringAngle, steeringSpeed * Time.fixedDeltaTime);

        // Apply steering to the front wheels.
        frontLeftWheel.steerAngle = currentSteeringAngle;
        frontRightWheel.steerAngle = currentSteeringAngle;

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


       
    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }
}
