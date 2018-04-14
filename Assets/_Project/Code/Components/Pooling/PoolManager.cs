using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	private static PoolManager instance;
	public static PoolManager Instance
	{
		get
		{
			if (instance == null)
				instance = FindObjectOfType<PoolManager> ();
			if (instance == null)
				instance = new GameObject ("PoolManager").AddComponent<PoolManager> ();
			return instance;
		}
	}

	private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>> ();

	public void CreatePool (GameObject prefab, int size)
	{
		var poolKey = prefab.GetInstanceID ();

		if (!poolDictionary.ContainsKey (poolKey))
			poolDictionary.Add (poolKey, new Queue<GameObject> ());

		for (int i = 0; i < size; i++)
		{
			var newObject = Instantiate (prefab) as GameObject;
			newObject.SetActive (false);

			poolDictionary[poolKey].Enqueue (newObject);
		}
	}

	public void ReuseObject (GameObject prefab, Vector3 position, Quaternion rotation)
	{
		var poolKey = prefab.GetInstanceID ();

		if (poolDictionary.ContainsKey (poolKey))
		{
			var objectToReuse = poolDictionary[poolKey].Dequeue ();
			poolDictionary[poolKey].Enqueue (objectToReuse);

			objectToReuse.SetActive (true);
			objectToReuse.transform.position = position;
			objectToReuse.transform.rotation = rotation;
		}
	}
}