using UnityEngine;
using UnityEngine.Tilemaps;

public class WinCondition : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] [TextArea]
	string description;
	#pragma warning restore 0649

	public bool Satisfied(TileTracker tracker) {
		foreach (GameStateRequirement g in GetComponents<GameStateRequirement>()) {
			if (!g.Satisfied(tracker)) {
				return false;
			}
		}

		return true;
	}

	public string GetDescription() {
		return description;
	}
}
