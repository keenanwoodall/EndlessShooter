using System.Collections;
using UnityEngine;

public class SetActiveDelayed : MonoBehaviour
{
	public GameObject target;
	public float delay;

	private IEnumerator Start ()
	{
		yield return new WaitForSeconds (delay);
		target.SetActive (true);
	}
}