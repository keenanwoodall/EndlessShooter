using UnityEngine;

public class ScaleByVelocity : MonoBehaviour
{
	public float bias = 1f;
	public float scale = 0.1f;
	public new Rigidbody2D rigidbody;

	private void Update ()
	{
		transform.localScale = Vector3.one * ((rigidbody.velocity.magnitude * scale) + bias);
	}
}