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
    Vector2 movement;
    //Turning
    public float steeringSpeed = 60f;
    public float currentSteeringAngle = 0f;
    public float lowSpeedTurnAngle = 35f;
    public float highSpeedTurnAngle = 8f;
    public float steeringReductionSpeed = 25f;
    
    //Braking
    public float brakeForce = 3000f;
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

        rb.mass = 1200f;
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
        currentSpeed = rb.linearVelocity.magnitude;
        float verticalInput = movement.y;
        float brakeInput = 1f;
        float motorTorque = 0f;

        //frontLeftWheel.brakeTorque = 0f;


        if (isBraking == true)
        {

            //brakeInput += 0.1f;
            //Debug.Log("shift pressed");
            //if (moveSpeed > 0f)
            //{
               Debug.Log("brake activates");
            //moveSpeed -= brakeInput;

            //}

            float brake = isBraking ? brakeForce : 0f;

            frontLeftWheel.brakeTorque = brake;
            frontRightWheel.brakeTorque = brake;
            rearLeftWheel.brakeTorque = brake;
            rearRightWheel.brakeTorque = brake;

        }
        else
        {
            //brakeInput = 1f;
            frontLeftWheel.brakeTorque = 0f;
            frontRightWheel.brakeTorque = 0f;
            rearLeftWheel.brakeTorque = 0f;
            rearRightWheel.brakeTorque = 0f;
        }
        if (verticalInput != 0f && moveSpeed < topSpeed)
        {
            //moveSpeed += Mathf.Abs(verticalInput);
            //Debug.Log("acceleration activates");
        }
        else if (moveSpeed > 0.3)
        {



            //rb.AddForce(-rb.linearVelocity.normalized * moveSpeed * brakeInput, ForceMode.Acceleration);

            //DragSpeed = previousSpeed - currentSpeed;



            //moveSpeed -= brakeInput;
            //Debug.Log("drag activates");
        }
        if (!isBraking && currentSpeed < topSpeed)
        {
            Debug.Log("acceleration activates, vertical:" + verticalInput + " force: " + force);
            motorTorque = verticalInput * force;
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
