using UnityEngine;

public class RotateTowardsVelocity : MonoBehaviour
{
	public float minVelocity = 0.1f;
	public Axis axis;
	public new Rigidbody2D rigidbody;

	private void Update ()
	{
		if (rigidbody == null || rigidbody.velocity.magnitude < minVelocity)
			return;

		var angle = -Vector2.SignedAngle (Vector2.up, rigidbody.velocity);

		var rotation = transform.localEulerAngles;
		switch (axis)
		{
			case Axis.X:
				rotation.x = angle;
				break;
			case Axis.Y:
				rotation.y = angle;
				break;
			case Axis.Z:
				rotation.z = angle;
				break;
		}

		transform.localEulerAngles = rotation;
	}
}