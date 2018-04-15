using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour
{
	public string horizontalAxis = "Move Horizontal";
	public string verticalAxis = "Move Vertical";
	public string dashButton = "Dash";

	public InputMovementEvent onMovementInput = new InputMovementEvent ();
	public InputDashEvent onDashInput = new InputDashEvent ();

	private void Update ()
	{
		var player = ReInput.players.GetPlayer ("Player");
		var input = new Vector2 (player.GetAxisRaw (horizontalAxis), player.GetAxisRaw (verticalAxis));
		input = Vector2.ClampMagnitude (input, 1f);

		onMovementInput.Invoke (input);

		if (player.GetButtonDown (dashButton) && input.sqrMagnitude > 0.1f)
			onDashInput.Invoke ();
	}
}