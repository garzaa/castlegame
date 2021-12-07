using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class MouseOverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

	public void OnPointerEnter(PointerEventData data) {
		animator.SetBool("MouseOver", true);
	}

	public void OnPointerExit(PointerEventData data) {
		animator.SetBool("MouseOver", false);
	}
}
