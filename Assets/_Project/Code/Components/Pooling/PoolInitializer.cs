using UnityEngine;

public class PoolInitializer : MonoBehaviour
{
	public GameObject prefab;
	public int count = 100;

	private void Start ()
	{
		PoolManager.Instance.CreatePool (prefab, count);
	}
}