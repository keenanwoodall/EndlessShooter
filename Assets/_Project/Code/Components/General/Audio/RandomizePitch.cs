using UnityEngine;

public class RandomizePitch : MonoBehaviour
{
	public bool onAwake;
	public float basePitch = 1f;
	public float offset = 0.1f;
	public AudioSource source;

	private void Awake ()
	{
		if (onAwake)
			Randomize ();
	}
	public void Randomize ()
	{
		source.pitch = Random.Range (basePitch - offset, basePitch + offset);
	}
}