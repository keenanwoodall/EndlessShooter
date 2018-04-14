using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public class ObjectInstance
	{
		GameObject instance;

		bool hasPoolObject;
		PoolObject poolObject;

		public ObjectInstance (GameObject instance)
		{
			this.instance = instance;
			poolObject = instance.GetComponent<PoolObject> ();
			hasPoolObject = poolObject != null;
		}

		public void Reuse (Vector3 position, Quaternion rotation)
		{
			instance.SetActive (true);
			instance.transform.position = position;
			instance.transform.rotation = rotation;

			if (hasPoolObject)
				poolObject.OnReuse ();
		}
	}

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

	private Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>> ();

	public void CreatePool (GameObject prefab, int size)
	{
		var poolKey = prefab.GetInstanceID ();

		if (!poolDictionary.ContainsKey (poolKey))
			poolDictionary.Add (poolKey, new Queue<ObjectInstance> ());

		for (int i = 0; i < size; i++)
		{
			var newObject = Instantiate (prefab) as GameObject;
			newObject.SetActive (false);

			poolDictionary[poolKey].Enqueue (new ObjectInstance (newObject));
		}
	}

	public void ReuseObject (GameObject prefab, Vector3 position, Quaternion rotation)
	{
		var poolKey = prefab.GetInstanceID ();

		if (poolDictionary.ContainsKey (poolKey))
		{
			var objectToReuse = poolDictionary[poolKey].Dequeue ();
			poolDictionary[poolKey].Enqueue (objectToReuse);

			objectToReuse.Reuse (position, rotation);
		}
	}
}