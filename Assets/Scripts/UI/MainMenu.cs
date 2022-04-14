using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {
	Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

	public void Exit() {
		Application.Quit();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			animator.SetTrigger("SkipLowerIn");
		}
	}
}
