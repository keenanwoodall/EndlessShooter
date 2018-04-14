using UnityEngine;

public class Damager : MonoBehaviour
{
	public float damage = 10f;
	public bool constant;

	private void OnTriggerEnter2D (Collider2D collision)
	{
		if (constant)
			return;

		var target = collision.gameObject.GetComponent<Damageable> ();
		if (target != null)
			target.ReceiveDamage (damage);
	}

	private void OnTriggerStay2D (Collider2D collision)
	{
		if (!constant)
			return;

		var target = collision.gameObject.GetComponent<Damageable> ();
		if (target != null)
			target.ReceiveDamage (damage * Time.deltaTime);
	}
}