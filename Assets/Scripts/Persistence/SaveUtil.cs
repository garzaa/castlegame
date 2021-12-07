using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class SaveUtil : MonoBehaviour {
	const string file = "save.json";

	string levelName;
	DayTracker dayTracker;
	Save save = new Save();

	void Awake() {
		dayTracker = GameObject.FindObjectOfType<DayTracker>();

		if (SaveExists()) {
			save = LoadSaveFromDisk();
		}

		levelName = SceneManager.GetActiveScene().name;
	}

	public bool HasLevelSaved(string levelName) {
		return save.HasLevelProgress(levelName);
	}

	public LevelProgress GetLevelProgress(string levelName) {
		return save.GetLevelProgress(levelName);
	}

	bool SaveExists() {
		if (File.Exists(SavePath())) {
			try {
				Save s = JsonUtility.FromJson<Save>(File.ReadAllText(SavePath()));
			} catch (Exception e) {
				Debug.Log(e.Message);
				return false;
			}
			return true;
		}
		return false;
	}

	string SavePath() {
		return Path.Combine(Application.persistentDataPath, file);
	}

	Save LoadSaveFromDisk() {
		return JsonUtility.FromJson<Save>(File.ReadAllText(SavePath()));
	}

	void WriteSaveToDisk(Save save) {
		File.WriteAllText(SavePath(), JsonUtility.ToJson(save));
	}

	public void OnGameWin() {
		int currentDays = dayTracker.GetTotalDays();
		int currentActions = dayTracker.GetTotalActions();

		if (save.HasLevelProgress(levelName)) {
			LevelProgress diskLevel = save.GetLevelProgress(levelName);
			if (currentActions<diskLevel.actions || (currentActions==diskLevel.actions && currentDays<diskLevel.days)) {
				save.SetLevelProgress(new LevelProgress(levelName, currentDays, currentActions));
			}

			WriteSaveToDisk(save);
		} else {
			save.SetLevelProgress(new LevelProgress(levelName, currentDays, currentActions));
			WriteSaveToDisk(save);
		}
	}
}
