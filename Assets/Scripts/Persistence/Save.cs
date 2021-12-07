using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Save {
	[SerializeField]
	List<LevelProgress> levels = new List<LevelProgress>();

	public bool HasLevelProgress(string levelName) {
		return GetLevelProgress(levelName) != null;
	}

	public LevelProgress GetLevelProgress(string levelName) {
		foreach (LevelProgress l in levels) {
			if (l.levelName.Equals(levelName)) {
				return l;
			}
		}
		return null;
	}

	public void SetLevelProgress(LevelProgress p) {
		for (int i=0; i<levels.Count; i++) {
			if (levels[i].levelName.Equals(p.levelName)) {
				levels[i] = p;
				return;
			}
		}
		levels.Add(p);
	}
}

[Serializable]
public class LevelProgress {
	public string levelName;
	public int days;
	public int actions;

	public LevelProgress(string n, int d, int a) {
		levelName = n;
		this.days = d;
		this.actions = a;
	}
}

