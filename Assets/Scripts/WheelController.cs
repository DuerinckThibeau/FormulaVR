using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{

    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField]Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    public float acceleration = 500000f;
    public float breakingForce = 3000f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBreakForce = 0f;
    private float currentTurnAngle = 0;


    private void FixedUpdate() {

      currentAcceleration = acceleration * Input.GetAxis("Vertical");

      if (Input.GetKey(KeyCode.Space))
        currentBreakForce = breakingForce;
      else
        currentBreakForce = 0f;

      frontRight.motorTorque = currentAcceleration;
      frontLeft.motorTorque = currentAcceleration;


      frontRight.brakeTorque = currentBreakForce;
      frontLeft.brakeTorque = currentBreakForce;
      backRight.brakeTorque = currentBreakForce;
      backLeft.brakeTorque = currentBreakForce;


      currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
      frontLeft.steerAngle = currentTurnAngle;
      frontRight.steerAngle = currentTurnAngle;
      
      UpdateWheel(frontRight, frontRightTransform);
      UpdateWheel(frontLeft, frontLeftTransform);
      UpdateWheel(backRight, backRightTransform);
      UpdateWheel(backLeft, backLeftTransform);
      

    }
    void UpdateWheel(WheelCollider col, Transform trans) {
      
      Vector3 position;
      Quaternion rotation;
      col.GetWorldPose(out position, out rotation);

      trans.position = position;
      trans.rotation = rotation;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
