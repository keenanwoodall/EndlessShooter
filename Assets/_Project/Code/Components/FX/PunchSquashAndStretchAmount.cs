using System.Collections;
using UnityEngine;
using Deform.Deformers;
using DG.Tweening;

public class PunchSquashAndStretchAmount : MonoBehaviour
{
	public float duration = 0.1f;
	public float to = 1f;
	public SquashAndStretchDeformer sns;

	public void Punch ()
	{
		var sequence = DOTween.Sequence ();
		sequence.Append (DOTween.To (() => 0f, v => sns.amount = v, to, duration * 0.5f).SetEase (Ease.Flash));
		sequence.Append (DOTween.To (() => to, v => sns.amount = v, 0f, duration * 0.5f).SetEase (Ease.Flash));
	}
}