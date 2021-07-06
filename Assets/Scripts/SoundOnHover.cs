using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class SoundOnHover : MonoBehaviour, IPointerEnterHandler {
	public AudioResource onHover;

	public void OnPointerEnter(PointerEventData data) {
		onHover.PlayFrom(this.gameObject);
	}
}
