using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class AudioResource : ScriptableObject {
	#pragma warning disable 0649
	[SerializeField] List<AudioClip> sounds;
	#pragma warning restore 0649

	public void PlayFrom(GameObject caller) {
		int idx = Random.Range(0, sounds.Count);
		caller.GetComponent<AudioSource>().PlayOneShot(sounds[idx]);
	}
}
