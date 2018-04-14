using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class EntityMotor : MonoBehaviour
{
	public float deaccelerationDuration = 0.1f;

	private new Rigidbody2D rigidbody;
	private Vector2 velocityRef;

	private void Awake ()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
		rigidbody.gravityScale = 0f;
		rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	private void Update ()
	{
		rigidbody.velocity = Vector2.SmoothDamp (rigidbody.velocity, Vector3.zero, ref velocityRef, deaccelerationDuration, float.MaxValue, Time.deltaTime);
	}

	public void AddVelocity (Vector2 velocity)
	{
		rigidbody.velocity += velocity;
	}
}