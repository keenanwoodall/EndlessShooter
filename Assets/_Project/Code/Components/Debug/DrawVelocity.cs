using UnityEngine;

public class DrawVelocity : MonoBehaviour
{
	public Color color = Color.red;
	public float lengthScalar = 1f;
	private new Rigidbody2D rigidbody;

	private void Awake ()
	{
		rigidbody = GetComponent<Rigidbody2D> ();
	}

	private void Update ()
	{
		Debug.DrawRay (transform.position, rigidbody.velocity * lengthScalar, color);
	}
}