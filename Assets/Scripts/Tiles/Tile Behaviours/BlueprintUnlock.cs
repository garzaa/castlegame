using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BlueprintUnlock : MonoBehaviour {
	bool unlocked = false;

	#pragma warning disable 0649
	[Tooltip("These should be children somewhere.")] 
	public List<GameStateRequirement> requirements;
	#pragma warning restore 0649

	public bool JustUnlocked(TileTracker tracker) {
		if (unlocked) return false;
		
		foreach (GameStateRequirement r in requirements) {
			if (!r.Satisfied(tracker)) return false;
		}
		unlocked = true;
		return true;
	}

	public bool Unlocked(TileTracker tracker) {
		foreach (GameStateRequirement r in requirements) {
			if (!r.Satisfied(tracker)) return false;
		}
		return true;
	}
}
