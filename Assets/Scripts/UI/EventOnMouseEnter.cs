using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventOnMouseEnter : MonoBehaviour, IPointerEnterHandler {
	#pragma warning disable 0649
	[SerializeField] UnityEvent target;

	public void OnPointerEnter(PointerEventData d) {
		target.Invoke();
	}
}
