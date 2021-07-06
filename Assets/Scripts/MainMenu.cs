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
		List<SceneReference> scenes = levels.GetLevels();
		for (int i=0; i<scenes.Count; i++) {
			string levelName = $"{i+1} - {levels.PathToLevelName(scenes[i].ScenePath)}";
		}
	}

	public void Exit() {
		Application.Quit();
	}
}
