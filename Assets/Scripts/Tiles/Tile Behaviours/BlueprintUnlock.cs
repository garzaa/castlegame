using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BlueprintUnlock : MonoBehaviour, ICardStat {
	bool unlocked = false;

	#pragma warning disable 0649
	[Tooltip("These should be children somewhere.")] 
	[SerializeField] List<GameStateRequirement> requirements;
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

	public string Stat() {
		string s = "Unlocked";
		foreach (GameStateRequirement requirement in requirements) {
			s += " " + requirement.ToString();
		}
		s += ".";
		return s;
	}
}
