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
		rigidbody.velocity = Vector3.zero;
	}

	private void Update ()
	{
		MotorUpdate ();
		rigidbody.velocity = Vector2.SmoothDamp (rigidbody.velocity, Vector3.zero, ref velocityRef, deaccelerationDuration, float.MaxValue, Time.deltaTime);
	}

	protected virtual void MotorUpdate () { }

	public void AddForce (Vector2 force)
	{
		if (rigidbody == null)
			rigidbody = GetComponent<Rigidbody2D> ();
		rigidbody.AddForce (force, ForceMode2D.Force);
	}
}