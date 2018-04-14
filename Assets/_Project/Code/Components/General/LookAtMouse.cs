using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
	public new Camera camera;

	private void Awake ()
	{
		if (camera == null)
			camera = Camera.main;
	}

	private void Update ()
	{
		var objectScreenPosition = camera.WorldToScreenPoint (transform.position);
		var mouseScreenPosition = Input.mousePosition;

		var direction = mouseScreenPosition - objectScreenPosition;
		var angle = Vector2.SignedAngle (Vector2.up, direction);

		var rotation = transform.localEulerAngles;
		rotation.z = angle;

		transform.localEulerAngles = rotation;
	}
}