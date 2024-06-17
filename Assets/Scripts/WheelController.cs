using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] TrackCheckpoints trackCheckpoints;


    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    public float maxAcceleration = 5e+10f;
    public float maxBreakingForce = 3e+10f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0f;

    public float accelerationRate = 1e+08f;
    public float brakingRate = 1e+08f;

    private Rigidbody rb;

    private void Start()
    {
        trackCheckpoints.OnPlayerWinRace += TrackCheckpoints_OnPlayerWinRace;

        rb = GetComponent<Rigidbody>();
        rb.mass = 1500f;
        rb.drag = 0.1f;
        rb.angularDrag = 0.1f;
    }

    private void TrackCheckpoints_OnPlayerWinRace(object sender, EventArgs e)
    {
        EndRace();
    }

    private void FixedUpdate()
    {
        //float verticalInput = Input.GetAxis("Vertical");
        //float turnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        //HandleMotor(verticalInput);
        //HandleSteering(turnAngle);
        UpdateWheels();
    }

    private void HandleMotor(float verticalInput)
    {
        // Smooth acceleration
        if (verticalInput > 0)
        {
            currentAcceleration = Mathf.MoveTowards(currentAcceleration, maxAcceleration, accelerationRate * Time.deltaTime);
        }
        else if (verticalInput < 0)
        {
            currentAcceleration = Mathf.MoveTowards(currentAcceleration, -maxAcceleration, accelerationRate * Time.deltaTime);
        }
        else
        {
            currentAcceleration = Mathf.MoveTowards(currentAcceleration, 0, accelerationRate * Time.deltaTime);
        }

        // Smooth braking
        if (Input.GetKey(KeyCode.Space))
        {
            currentBreakForce = Mathf.MoveTowards(currentBreakForce, maxBreakingForce, brakingRate * Time.deltaTime);
        }
        else
        {
            currentBreakForce = Mathf.MoveTowards(currentBreakForce, 0, brakingRate * Time.deltaTime);
        }

        frontRight.motorTorque = currentAcceleration;
        frontLeft.motorTorque = currentAcceleration;

        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;
    }

    private void HandleSteering(float turnAngle)
    {
        currentTurnAngle = maxTurnAngle * turnAngle;
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
    }

    private void UpdateWheels()
    {
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(backRight, backRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    public void StopCompletely() {
        currentAcceleration = 0;
        currentBreakForce = 0;
        currentTurnAngle = 0;


        frontRight.motorTorque = 0;
        frontLeft.motorTorque = 0;

        frontRight.brakeTorque = 0;
        frontLeft.brakeTorque = 0;
        backRight.brakeTorque = 0;
        backLeft.brakeTorque = 0;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void EndRace() {
        Time.timeScale = 0;   
    } 

    internal void SetInputs(float forwardAmount, float turnAmount)
    {
        HandleMotor(forwardAmount);
        HandleSteering(turnAmount);
    }
}

