using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject levelContainer;
	[SerializeField] GameObject levelTemplate;
	#pragma warning restore 0649

	Levels levels;
	Animator animator;

	const string levelsShown = "LevelsShown";

	void Start() {
		animator = GetComponent<Animator>();
		levels = GameObject.FindObjectOfType<Levels>();
		PopulateLevels();
	}

	public void LoadLevel(int levelNum) {
		levels.LoadLevel(levelNum);
	}

	public void ToggleLevels() {
		animator.SetBool(levelsShown, !animator.GetBool(levelsShown));
	}

	public void ShowLevels() {
		animator.SetBool(levelsShown, true);
	}

	public void HideLevels() {
		animator.SetBool(levelsShown, false);
	}

	public void PopulateLevels() {

	}

	public void Exit() {
		Application.Quit();
	}
}
