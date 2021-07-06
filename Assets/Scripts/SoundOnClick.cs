using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class SoundOnClick: MonoBehaviour, IPointerDownHandler {
	public AudioResource onClick;

	public void OnPointerDown(PointerEventData data) {
		onClick.PlayFrom(this.gameObject);
	}
}
