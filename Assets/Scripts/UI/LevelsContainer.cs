using UnityEngine;

public class LevelsContainer : MonoBehaviour {
	Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

	public void ToggleLevels() {
		animator.SetBool("Shown", !animator.GetBool("Shown"));
	}
}
