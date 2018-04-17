using System.Collections;
using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
	public float radius = 20f;
	public float delay = 10f;
	public float delayDecay = 0.5f;
	public float minDelay = 0.1f;
	public GameObject prefab;

	private void Start ()
	{
		StartCoroutine (SpawnRoutine ());
	}

	private IEnumerator SpawnRoutine ()
	{
		while (true)
		{
			var position = Random.insideUnitCircle.normalized * radius;
			Instantiate (prefab, position, Quaternion.identity);

			yield return new WaitForSeconds (delay);

			delay -= delayDecay * delay;
			if (delay < minDelay)
				delay = minDelay;
		}
	}
}