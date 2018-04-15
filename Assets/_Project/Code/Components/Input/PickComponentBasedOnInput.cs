using UnityEngine;
using Rewired;

public class PickComponentBasedOnInput : MonoBehaviour
{
	public Behaviour a, b;

	public string[] aInputAxes;
	public string[] bInputAxes;

	private void Update ()
	{
		var player = ReInput.players.GetPlayer ("Player");
		print (player.GetAxisRaw ("Move Mouse Horizontal"));
		foreach (var axis in aInputAxes)
		{
			if (Mathf.Abs (player.GetAxisRaw (axis)) > 0.01f)
			{
				a.enabled = true;
				b.enabled = false;
				return;
			}
		}
		foreach (var axis in bInputAxes)
		{
			if (Mathf.Abs (player.GetAxisRaw (axis)) > 0.01f)
			{
				b.enabled = true;
				a.enabled = false;
				return;
			}
		}
	}
}