using UnityEngine;

public class ObjectInstanceSpawner : MonoBehaviour
{
	public GameObject prefab;

	public void Spawn ()
	{
		PoolManager.Instance.ReuseObject (prefab, transform.position, transform.rotation * prefab.transform.rotation);
	}
}