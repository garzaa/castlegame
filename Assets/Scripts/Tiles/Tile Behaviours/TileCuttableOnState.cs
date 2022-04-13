using UnityEngine;

public class TileCuttableOnState : TileCuttable {
	public GameStateRequirement[] stateRequirements;

	public override bool Cuttable() {
		foreach (GameStateRequirement r in stateRequirements) {
			if (!r.Satisfied(gameTile.GetTracker())) {
				return false;
			}
		}
		return true;
	}
}
