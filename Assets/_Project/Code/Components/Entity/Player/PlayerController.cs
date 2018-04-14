using UnityEngine;

[RequireComponent (typeof (EntityMotor))]
public class PlayerController : MonoBehaviour
{
	public float speed = 5f;
	public float dashVelocity = 50f;
	public float dashDuration = 0.1f;
	public float dashCooldownDuration = 0.5f;

	private EntityMotor motor;
	private Vector2 lastInput;
	private float lastDashTime;

	private void Awake ()
	{
		motor = GetComponent<EntityMotor> ();
	}

	public void HandleMovementInput (Vector2 input)
	{
		motor.AddVelocity (input * speed);
		lastInput = input;
	}

	public void HandleDashInput ()
	{
		if (CanDash ())
			Dash ();
	}

	private bool CanDash ()
	{
		return Time.time - lastDashTime > dashCooldownDuration;
	}

	private void Dash ()
	{
		motor.AddVelocity (lastInput * dashVelocity);
		lastDashTime = Time.time;
	}
}