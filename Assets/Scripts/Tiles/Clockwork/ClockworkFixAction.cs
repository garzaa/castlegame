using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Clockwork/Fix Action")]
public class ClockworkFixAction : ExclusiveClockworkAction {
	public int limit = -1;
	public bool onlyStructures;

	public override void ExecuteApply(ClockworkApply action) {
		List<GameTile> toFix = action.targets
			.Where(x=> x.GetComponent<TileAge>())
			.OrderBy(x => GetNearestDecay(x))
			.ToList();
		
		if (limit > 0) {
			for (int i=0; i<limit; i++) {
				RepairTile(toFix[i], action.sourceTile);
			}
		} else {
			foreach (GameTile t in toFix) {
				RepairTile(t, action.sourceTile);
			}
		}
	}

	void RepairTile(GameTile targetTile, GameTile sourceTile) {
		TileAge a = targetTile.GetComponent<TileAge>();
		if (a.GetAge() < 1) return;
		if (onlyStructures && !targetTile.GetComponent<StructureTile>()) return;

		a.Repair();
		TileTracker tracker = targetTile.GetTracker();
		string msg = $"{sourceTile.name} at {tracker.PosToStr(sourceTile.position)}";
		msg += $" repaired {targetTile.name} at {tracker.PosToStr(targetTile.position)}";
		CommandInput.Log(msg);
	}

	float GetNearestDecay(GameTile tile) {
		// doesn't account for multipliers but whatever
		// maybe neighbor decay should just alter the base multiplier
		int decayWindow = int.MaxValue;

		foreach (TileDecay d in tile.GetComponents<TileDecay>()) {
			decayWindow = Mathf.Min(d.GetDecayThreshold() - d.GetDecay(), decayWindow);
		}

		return decayWindow;
	}
}
