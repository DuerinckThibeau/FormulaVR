using UnityEngine;

public class CameraRotationLimiter : MonoBehaviour
{
	public float mouseSensitivity = 100f;
	public Transform cameraTransform;  // Verwijst naar de camera
	public Transform cameraOffsetTransform;  // Verwijst naar de Camera Offset
	public float maxRotationX = 90f;   // Maximum rotatie naar boven/beneden
	public float minRotationX = -90f;  // Minimum rotatie naar boven/beneden

	private float xRotation = 0f;
	private float yRotation = 0f;
	private bool isRightMouseButtonPressed = false;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		// Check if the right mouse button is pressed or released
		if (Input.GetMouseButtonDown(1))
		{
			isRightMouseButtonPressed = true;
		}
		else if (Input.GetMouseButtonUp(1))
		{
			isRightMouseButtonPressed = false;
		}

		// Rotate the camera only when the right mouse button is pressed
		if (isRightMouseButtonPressed)
		{
			float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
			float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

			yRotation += mouseX;
			xRotation -= mouseY;

			// Clamp the rotations
			xRotation = Mathf.Clamp(xRotation, minRotationX, maxRotationX);

			// Apply the rotations to the camera offset
			cameraOffsetTransform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
			cameraTransform.localRotation = cameraOffsetTransform.localRotation;
		}
	}
}
