using UnityEngine;

public class ObjectInstanceSpawner : MonoBehaviour
{
	public GameObject prefab;

	public void Spawn ()
	{
		Instantiate (prefab, transform.position, transform.rotation * prefab.transform.rotation);
	}
}