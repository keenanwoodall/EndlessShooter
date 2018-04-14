using UnityEngine;

public class PlayerGun : MonoBehaviour
{
	public float cooldownDuration = 0.1f;
	public GameObject projectilePrefab;

	private float lastShotTime;

	private void Start ()
	{
		PoolManager.Instance.CreatePool (projectilePrefab, 200);
	}

	private void Update ()
	{
		if (Input.GetButton ("Shoot"))
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
	}
}