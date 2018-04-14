using System.Collections;
using UnityEngine;

[RequireComponent (typeof (EntityMotor))]
public class PlayerController : MonoBehaviour
{
	public float speed = 5f;
	public float dashSpeedScalar = 50f;
	public float dashDuration = 0.1f;
	public float dashCooldownDuration = 0.5f;

	private EntityMotor motor;
	private Vector2 lastInput;
	private float lastDashTime;
	private Coroutine dashRoutine;

	private void Awake ()
	{
		motor = GetComponent<EntityMotor> ();
	}

	public void HandleMovementInput (Vector2 input)
	{
		motor.SetTargetVelocity (input * speed);
		lastInput = input;
	}

	public void HandleDashInput ()
	{
		if (CanDash ())
		{
			if (dashRoutine != null)
				StopCoroutine (dashRoutine);

			dashRoutine = StartCoroutine (DashRoutine ());
		}
	}

	private bool CanDash ()
	{
		return Time.time - lastDashTime > dashCooldownDuration;
	}

	private IEnumerator DashRoutine ()
	{
		motor.SetVelocityScalar (dashSpeedScalar);

		yield return new WaitForSeconds (dashDuration);

		motor.SetVelocityScalar (1f);
		lastDashTime = Time.time;
	}
}