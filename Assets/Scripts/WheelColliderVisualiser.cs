using UnityEngine;

[ExecuteInEditMode]
public class WheelColliderVisualizer : MonoBehaviour
{
	public Color gizmoColor = Color.green;

	private WheelCollider wheelCollider;

	void OnDrawGizmos()
	{
		wheelCollider = GetComponent<WheelCollider>();

		if (wheelCollider != null)
		{
			// Visualize the WheelCollider
			Vector3 wheelWorldPos;
			Quaternion wheelWorldRot;
			wheelCollider.GetWorldPose(out wheelWorldPos, out wheelWorldRot);

			Gizmos.color = gizmoColor;
			Gizmos.DrawWireSphere(wheelWorldPos, wheelCollider.radius);
		}
	}
}
