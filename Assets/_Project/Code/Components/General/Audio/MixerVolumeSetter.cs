using UnityEngine;
using UnityEngine.Audio;

public class MixerVolumeSetter : MonoBehaviour
{
	public AudioMixer mixer;

	public void SetVolume (float volume)
	{
		mixer.SetFloat ("Volume", (volume - 1f) * 80f);
	}
}