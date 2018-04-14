using UnityEngine;

public class Damageable : MonoBehaviour
{
	public float health = 100;

	public DamagedEvent onDamaged = new DamagedEvent ();
	public KilledEvent onKilled = new KilledEvent ();

	public void ReceiveDamage (float amount)
	{
		health -= amount;

		onDamaged.Invoke (amount);

		if (health <= 0)
		{
			health = 0;
			onKilled.Invoke ();
		}
	}
}