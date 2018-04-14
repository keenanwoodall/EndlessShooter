using UnityEngine;
using UnityEngine.Events;

public class PoolObject : MonoBehaviour
{
	public UnityEvent onReuse = new UnityEvent ();

	public virtual void OnReuse ()
	{
		onReuse.Invoke ();
	}

	public void Destroy ()
	{
		gameObject.SetActive (false);
	}
}