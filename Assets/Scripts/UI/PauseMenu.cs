using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	Levels levels;

	#pragma warning disable 0649
	[SerializeField] GameObject pauseUI;
	#pragma warning restore 0649

	void Start() {
		levels = GameObject.FindObjectOfType<Levels>();
		pauseUI.gameObject.SetActive(false);
	}

	public void NextLevel() {
		levels.LoadNextLevel();
	}

	public void ReloadLevel() {
		levels.ReloadLevel();
	}

	public void Exit() {
		Application.Quit();
	}

	public void MainMenu() {
		levels.LoadMainMenu();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			pauseUI.SetActive(!pauseUI.activeSelf);
		}
	}
}
