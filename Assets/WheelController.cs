using UnityEngine;

public class WheelController : MonoBehaviour
{
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
		rb = GetComponentInParent<Rigidbody>();
		if (rb == null)
		{
			Debug.LogError("Rigidbody not found in parent objects.");
			return;
		}
		rb.mass = 1500f; // Adjust the mass as needed
		rb.drag = 0.1f; // Adjust drag to simulate air resistance
		rb.angularDrag = 0.1f; // Adjust angular drag as needed

		// Ensure the WheelColliders have the correct friction settings
		SetupWheelColliders();

		// Position WheelColliders to match visual wheels
		PositionWheelColliders();
	}

	private void SetupWheelColliders()
	{
		WheelFrictionCurve forwardFriction = new WheelFrictionCurve();
		forwardFriction.extremumSlip = 1;
		forwardFriction.extremumValue = 5000;
		forwardFriction.asymptoteSlip = 2;
		forwardFriction.asymptoteValue = 2500;
		forwardFriction.stiffness = 1;

		WheelFrictionCurve sidewaysFriction = new WheelFrictionCurve();
		sidewaysFriction.extremumSlip = 1;
		sidewaysFriction.extremumValue = 5000;
		sidewaysFriction.asymptoteSlip = 2;
		sidewaysFriction.asymptoteValue = 2500;
		sidewaysFriction.stiffness = 1;

		frontRight.forwardFriction = forwardFriction;
		frontLeft.forwardFriction = forwardFriction;
		backRight.forwardFriction = forwardFriction;
		backLeft.forwardFriction = forwardFriction;

		frontRight.sidewaysFriction = sidewaysFriction;
		frontLeft.sidewaysFriction = sidewaysFriction;
		backRight.sidewaysFriction = sidewaysFriction;
		backLeft.sidewaysFriction = sidewaysFriction;
	}

	private void PositionWheelColliders()
	{
		PositionWheelCollider(frontRight, frontRightTransform);
		PositionWheelCollider(frontLeft, frontLeftTransform);
		PositionWheelCollider(backRight, backRightTransform);
		PositionWheelCollider(backLeft, backLeftTransform);
	}

	private void PositionWheelCollider(WheelCollider collider, Transform transform)
	{
		collider.transform.position = transform.position;
		collider.transform.rotation = transform.rotation;
	}

	private void FixedUpdate()
	{
		if (rb == null) return; // Ensure Rigidbody is assigned

		HandleMotor();
		HandleSteering();
		UpdateWheels();
	}

	private void HandleMotor()
	{
		float verticalInput = Input.GetAxis("Vertical");

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
			currentAcceleration = Mathf.MoveTowards(currentAcceleration,
				0, accelerationRate * Time.deltaTime);
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

	private void HandleSteering()
	{
		currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
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
}
