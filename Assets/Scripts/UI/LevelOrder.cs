using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOrder : ScriptableObject {
	#pragma warning disable 0649
	[SerializeField] List<SceneReference> levels;
	#pragma warning restore 0649

	public List<SceneReference> GetLevels() {
		return levels;
	}
}
