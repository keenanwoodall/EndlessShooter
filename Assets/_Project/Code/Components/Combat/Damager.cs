using UnityEngine;

public class Damager : MonoBehaviour
{

	public bool onTrigger = true;
	public float damage = 10f;
	public bool addForce = true;
	public float force = 5f;
	public bool constant;

	private void OnCollisionEnter2D (Collision2D collision)
	{
		if (constant || onTrigger || SameLayer (collision))
			return;

		var target = collision.gameObject.GetComponent<Damageable> ();
		if (target != null)
			target.ReceiveDamage (damage);

		if (addForce)
		{
			var motor = collision.gameObject.GetComponent<EntityMotor> ();
			if (motor != null)
				motor.AddForce (transform.up * force);
		}
	}
	private void OnCollisionStay2D (Collision2D collision)
	{
		if (!constant || onTrigger || SameLayer (collision))
			return;

		var target = collision.gameObject.GetComponent<Damageable> ();
		if (target != null)
			target.ReceiveDamage (damage * Time.deltaTime);
	}
	private void OnTriggerEnter2D (Collider2D collision)
	{
		if (constant || !onTrigger || SameLayer (collision))
			return;

		var target = collision.gameObject.GetComponent<Damageable> ();
		if (target != null)
			target.ReceiveDamage (damage);

		if (addForce)
		{
			var motor = collision.gameObject.GetComponent<EntityMotor> ();
			if (motor != null)
				motor.AddForce (transform.up * force);
		}
	}

	private void OnTriggerStay2D (Collider2D collision)
	{
		if (!constant || !onTrigger || SameLayer (collision))
			return;

		var target = collision.gameObject.GetComponent<Damageable> ();
		if (target != null)
			target.ReceiveDamage (damage * Time.deltaTime);
	}

	private bool SameLayer (Collider2D collision)
	{
		return collision.gameObject.layer == gameObject.layer;
	}
	private bool SameLayer (Collision2D collision)
	{
		return collision.gameObject.layer == gameObject.layer;
	}
}