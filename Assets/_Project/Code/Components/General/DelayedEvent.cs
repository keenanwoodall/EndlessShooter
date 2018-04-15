using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
	public bool onAwake;
	public float delay = 1f;
	public UnityEvent onDelayFinish = new UnityEvent ();

	private Coroutine delayRoutine;


	private void Awake ()
	{
		if (onAwake)
			Initialize ();
	}

	public void Initialize ()
	{
		if (delayRoutine != null)
			return;

		delayRoutine = StartCoroutine (DelayRoutine ());
	}

	private IEnumerator DelayRoutine ()
	{
		yield return new WaitForSeconds (delay);

		onDelayFinish.Invoke ();

		delayRoutine = null;
	}
}