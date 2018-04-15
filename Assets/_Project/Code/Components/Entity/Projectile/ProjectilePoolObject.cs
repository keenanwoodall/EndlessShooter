using UnityEngine;

public class ProjectilePoolObject : PoolObject
{
	private new Rigidbody2D rigidbody;

	private void Awake ()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	public override void OnReuse ()
	{
		rigidbody.velocity = Vector3.zero;
	}

	private void OnDisable ()
	{
		rigidbody.velocity = Vector3.zero;
	}
}