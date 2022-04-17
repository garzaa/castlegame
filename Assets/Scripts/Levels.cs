using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Levels : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] SceneReference mainMenu;
	[SerializeField] LevelOrder levelOrder;
	#pragma warning restore 0649

	TransitionManager transitionManager;
	List<SceneReference> levels;

	void Awake() {
		levels = levelOrder.GetLevels();
	}

	void Start() {
		transitionManager = GameObject.FindObjectOfType<TransitionManager>();
	}

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
		transitionManager.LoadSceneImmediately(SceneManager.GetActiveScene().name);
	}

	public bool HasNextLevel() {
		return GetLevelNumber() < levels.Count-1;
	}

	public void LoadNextLevel() {
		transitionManager.LoadScene(levels[GetLevelNumber()+1]);
	}

	public void LoadMainMenu() {
		transitionManager.LoadScene(mainMenu);
	}

	public void LoadLevel(int levelNumber) {
		transitionManager.LoadScene(levels[levelNumber]);
	}

	public List<SceneReference> GetLevels() {
		return levels;
	}

	public string PathToLevelName(string path) {
		string[] p = path.Split('/');
		return p[p.Length-1].Split('.')[0];
	}
}
