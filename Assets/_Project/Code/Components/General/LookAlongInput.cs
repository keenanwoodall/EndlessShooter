using UnityEngine;
using Rewired;

public class LookAlongInput : MonoBehaviour
{
	public string xAxis = "Look Horizontal";
	public string yAxis = "Look Vertical";

	private void Update ()
	{
		var player = ReInput.players.GetPlayer ("Player");

		var direction = new Vector2 (player.GetAxisRaw (xAxis), player.GetAxisRaw (yAxis)).normalized;
		if (Mathf.Approximately (direction.sqrMagnitude, 0f))
			return;
		var angle = Vector2.SignedAngle (Vector2.up, direction);

		var rotation = transform.localEulerAngles;
		rotation.z = angle;
		transform.localEulerAngles = rotation;
	}
}