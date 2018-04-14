using UnityEngine;

[RequireComponent (typeof (EntityMotor))]
public class ProjectileController : MonoBehaviour
{
	public float speed = 20f;

	protected EntityMotor motor;

	private void Awake ()
	{
		motor = GetComponent<EntityMotor> ();
	}

	private void Update ()
	{
		ProjectileUpdate ();
	}

	protected virtual void ProjectileUpdate ()
	{
		motor.AddForce (transform.up * speed * Time.deltaTime);
	}
}