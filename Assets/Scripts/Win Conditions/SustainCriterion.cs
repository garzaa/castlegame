using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class SustainCriterion : WinConditionCriterion {
	public int days;

	public override bool Satisfied(TileTracker tracker) {
		return CommandInput.GetDaysWithoutActions() >= days;
	}
}
