using UnityEngine;

public class EventAudio : MonoBehaviour {
	public AudioResource audioResource;

	public void Play() {
		audioResource.PlayFrom(GetComponentInParent<AudioSource>().gameObject);
	}
}
