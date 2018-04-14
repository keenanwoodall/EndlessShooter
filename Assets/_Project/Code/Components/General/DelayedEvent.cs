using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
	public float delay = 1f;
	public UnityEvent onDelayFinish = new UnityEvent ();

	private Coroutine delayRoutine;

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