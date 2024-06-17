using UnityEngine;

public class CarMovement : MonoBehaviour
{
	public float speed = 10f;
	public float rotationSpeed = 100f;

	void Update()
	{
		// Voorwaartse en achterwaartse beweging
		float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		transform.Translate(0, 0, move);

		// Rotatie naar links en rechts
		float rotate = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
		transform.Rotate(0, rotate, 0);
	}
}


