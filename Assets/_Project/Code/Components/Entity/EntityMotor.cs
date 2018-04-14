using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class EntityMotor : MonoBehaviour
{
	public float accelerationDuration = 0.1f;

	private new Rigidbody2D rigidbody;
	private float velocityScalar = 1f;
	private Vector2 targetVelocity;
	private Vector2 targetVelocityRef;

	private void Awake ()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
		rigidbody.gravityScale = 0f;
		rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	private void Update ()
	{
		rigidbody.velocity = Vector2.SmoothDamp (rigidbody.velocity, targetVelocity * velocityScalar, ref targetVelocityRef, accelerationDuration, float.MaxValue, Time.deltaTime);
	}

	public void SetTargetVelocity (Vector2 velocity)
	{
		targetVelocity = velocity;
	}

	public void SetVelocityScalar (float scale)
	{
		velocityScalar = scale;
	}
}