using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ContainsTile : WinConditionCriterion {
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
}