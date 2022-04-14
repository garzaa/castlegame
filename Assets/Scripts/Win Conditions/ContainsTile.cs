using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ContainsTile : GameStateRequirement {
	public enum ComparisonType {
		EQ_OR_GT = 0,
		EXACTLY = 1
	}

	public ScriptableTile tile;
	public int number = 1;
	public ComparisonType comparisonType;

	public override bool Satisfied(TileTracker tracker) {
		int count = tracker.ContainsTile(tile);
		if (comparisonType.Equals(ComparisonType.EQ_OR_GT)) {
			return count >= number;
		} else {
			return count == number;
		}
	}

	public override string ToString() {
		if (number == 1) {
			return $"with a <color=\"#94fdff\">{tile.name}</color> on the board";
		} else {
			return $"with {number} <color=\"#94fdff\">{tile.name}s</color> on the board";
		}
	}
}
