using UnityEngine;

public class TileCuttableOnState : TileCuttable {
	public GameStateRequirement stateRequirement;

	public override bool Cuttable() {
		return stateRequirement.Satisfied(gameTile.GetTracker());
	}

	public override string Stat() {
		TileCuttableOnState t = GetComponent<TileCuttableOnState>();
		if (t && !t.Cuttable()) {
			return "";
		}
		// Cuttable to x.
		string s = base.Stat();
		// Cuttable to x [state].
		return s.Substring(0, s.Length-1) + " " + stateRequirement.ToString() + ".";
	}
}
