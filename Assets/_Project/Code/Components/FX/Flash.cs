using UnityEngine;
using DG.Tweening;

public class Flash : MonoBehaviour
{
	public float duration = 0.1f;
	public new MeshRenderer renderer;
	private Material material;

	private void Awake ()
	{
		material = renderer.material;
	}

	public void DoFlash ()
	{
		var s = DOTween.Sequence ();
		s.Append (material.DOFloat (1f, "_FlashStrength", duration * 0.5f).SetEase (Ease.Flash));
		s.Append (material.DOFloat (0f, "_FlashStrength", duration * 0.5f).SetEase (Ease.Flash));
	}
}