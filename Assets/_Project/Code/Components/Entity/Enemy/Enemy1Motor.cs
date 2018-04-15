using UnityEngine;

public class Enemy1Motor : EntityMotor
{
	public static Transform Target;

	public float speed = 20f;

	private void Start ()
	{
		if (Target == null)
			Target = FindObjectOfType<PlayerController> ().transform;
	}

	protected override void MotorUpdate ()
	{
		base.MotorUpdate ();
		var direction = (Target.position - transform.position).normalized;

		AddForce (direction * speed * Time.deltaTime);
	}
}