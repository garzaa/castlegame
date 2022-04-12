using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventOnMouseExit : MonoBehaviour, IPointerExitHandler {
	#pragma warning disable 0649
	[SerializeField] UnityEvent target;

	public void OnPointerExit(PointerEventData d) {
		target.Invoke();
	}
}
