using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class EntityMotor : MonoBehaviour
{
	public float speed = 10f;
	public float accelerationDuration = 0.2f;

	private new Rigidbody2D rigidbody;
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
		rigidbody.velocity = Vector2.SmoothDamp (rigidbody.velocity, targetVelocity * speed, ref targetVelocityRef, accelerationDuration, float.MaxValue, Time.deltaTime);
	}

	public void SetTargetVelocity (Vector2 velocity)
	{
		targetVelocity = velocity;
	}
}