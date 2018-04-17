using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
	public void Reload ()
	{
		SceneManager.LoadScene (0);
	}
}