using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class AudioResource : ScriptableObject {
	#pragma warning disable 0649
	[SerializeField] List<AudioClip> sounds;
	#pragma warning restore 0649

	public void PlayFrom(GameObject caller) {
		int idx = Random.Range(0, sounds.Count);
		AudioSource callerSource = caller.GetComponent<AudioSource>();
		if (callerSource == null) {
			callerSource = caller.AddComponent<AudioSource>();
		}
		callerSource.PlayOneShot(sounds[idx], callerSource.volume);
	}
}
