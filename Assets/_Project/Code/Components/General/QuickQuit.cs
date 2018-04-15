using UnityEngine;

public class QuickQuit : MonoBehaviour
{
	private void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
	}
}