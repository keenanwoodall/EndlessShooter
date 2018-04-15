using Rewired;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGun : MonoBehaviour
{
	public bool keepShooting;
	public float cooldownDuration = 0.1f;
	public GameObject projectilePrefab;
	public AudioSource shootSound;
	public UnityEvent onShoot = new UnityEvent ();


	private float lastShotTime;

	private void Update ()
	{
		var player = ReInput.players.GetPlayer ("Player");
		if (player.GetButton ("Shoot") || keepShooting)
			if (CanShoot ())
				Shoot ();
	}

	private bool CanShoot ()
	{
		return Time.time - lastShotTime > cooldownDuration;
	}

	private void Shoot ()
	{
		Instantiate (projectilePrefab, transform.position, transform.rotation);
		shootSound.Play ();
		lastShotTime = Time.time;

		onShoot.Invoke ();
	}
}