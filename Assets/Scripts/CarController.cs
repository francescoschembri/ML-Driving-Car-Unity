using UnityEngine;

public class CarController : MonoBehaviour
{
    private float steering;
    private float acceleration;
    private bool isBraking;

    public float motorForce = 1000;
    public float brakeForce = 3000;
    public float maxSteerAngle = 10;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    private Rigidbody rb;
    public Vector3 centerOfMass;

    [Header("IA parameters")]
    public bool hasCrashed = false;
    public float minVelocitiy = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
    }

    void FixedUpdate()
    {
        HandleSteering();
        HandleMotor();
        UpdateWheels();
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = acceleration * motorForce;
        frontRightWheelCollider.motorTorque = acceleration * motorForce;
        ApplyBraking();
    }

    private void ApplyBraking()
    {
        float currentBrakeForce = isBraking ? brakeForce : 0f;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        float currentSteerAngle = Mathf.LerpAngle(maxSteerAngle * steering, frontRightWheelCollider.steerAngle, Time.fixedDeltaTime);
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    public void SetSteering(float s)
    {
        steering = s;
    }

    public void SetAcceleration(float a)
    {
        if (a < 0f)
            isBraking = true;
        acceleration = Mathf.Clamp01(a);
    }

    public void Brake()
    {
        acceleration = 0f;
        isBraking = true;
    }

    public bool isMoving()
    {
        return rb.velocity.magnitude > minVelocitiy;
    }

    public void ResetCar()
    {
        isBraking = false;
        acceleration = 0f;
        steering = 0f;
        hasCrashed = false;
        frontLeftWheelCollider.steerAngle = 0f;
        frontRightWheelCollider.steerAngle = 0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
            hasCrashed = true;
    }
}
