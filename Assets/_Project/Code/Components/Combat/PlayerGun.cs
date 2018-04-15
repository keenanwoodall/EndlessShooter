using Rewired;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGun : MonoBehaviour
{
	public float cooldownDuration = 0.1f;
	public GameObject projectilePrefab;
	public UnityEvent onShoot = new UnityEvent ();


	private float lastShotTime;

	private void Update ()
	{
		var player = ReInput.players.GetPlayer ("Player");
		if (player.GetButton ("Shoot"))
			if (CanShoot ())
				Shoot ();
	}

	private bool CanShoot ()
	{
		return Time.time - lastShotTime > cooldownDuration;
	}

	private void Shoot ()
	{
		PoolManager.Instance.ReuseObject (projectilePrefab, transform.position, transform.rotation);
		lastShotTime = Time.time;

		onShoot.Invoke ();
	}
}