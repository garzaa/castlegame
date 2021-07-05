using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Levels : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] SceneReference mainMenu;
	[SerializeField] List<SceneReference> levels;
	#pragma warning restore 0649

	public int GetLevelNumber() {
		int levelNumber = 0;
		for (int i=0; i<levels.Count; i++) {
			if (levels[i].ScenePath.Equals(SceneManager.GetActiveScene().path)) {
				levelNumber = i;
				break;
			}
		}
		return levelNumber;
	}

	public void ReloadLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public bool HasNextLeve() {
		return GetLevelNumber() < levels.Count-1;
	}

	public void LoadNextLevel() {
		SceneManager.LoadScene(levels[GetLevelNumber()+1]);
	}

	public void LoadMainMenu() {
		SceneManager.LoadScene(mainMenu);
	}
}
