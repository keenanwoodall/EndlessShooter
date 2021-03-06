﻿using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (EntityMotor))]
public class PlayerController : MonoBehaviour
{
	public float speed = 5f;
	public float dashVelocity = 50f;
	public float dashDuration = 0.1f;
	public float dashCooldownDuration = 0.5f;

	public UnityEvent onDash = new UnityEvent ();

	private EntityMotor motor;
	private Vector2 lastInput;
	private float lastDashTime;

	private void Awake ()
	{
		motor = GetComponent<EntityMotor> ();
	}

	public void HandleMovementInput (Vector2 input)
	{
		motor.AddForce (input * speed * Time.deltaTime);
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
		motor.AddForce (lastInput * dashVelocity);
		lastDashTime = Time.time;
		onDash.Invoke ();
	}
}