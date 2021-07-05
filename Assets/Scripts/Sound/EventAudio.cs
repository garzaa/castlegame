using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EventAudio : MonoBehaviour {
	public AudioResource audioResource;

	public void Play() {
		audioResource.PlayFrom(this.gameObject);
	}
}
