using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class SustainCriterion : GameStateRequirement {
	public int days;
	DayTracker dayTracker;

	public override bool Satisfied(TileTracker tracker) {
		if (!dayTracker) dayTracker = GameObject.FindObjectOfType<DayTracker>();
		return dayTracker.GetDaysWithoutActions() >= days;
	}
}
