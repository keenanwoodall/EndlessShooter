using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";
	public string dashButton = "Dash";

	public InputMovementEvent onMovementInput = new InputMovementEvent ();
	public InputDashEvent onDashInput = new InputDashEvent ();

	private void Update ()
	{
		var input = new Vector2 (Input.GetAxisRaw (horizontalAxis), Input.GetAxisRaw (verticalAxis));
		input = Vector2.ClampMagnitude (input, 1f);

		onMovementInput.Invoke (input);

		if (Input.GetButtonDown (dashButton))
			onDashInput.Invoke ();
	}
}