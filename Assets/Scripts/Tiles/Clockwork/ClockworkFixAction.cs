using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Clockwork/Fix Action")]
public class ClockworkFixAction : ExclusiveClockworkAction {
	public int limit = -1;

	public override void ExecuteApply(ClockworkApply action) {
		List<GameTile> toFix = action.targets
			.Where(x=> x.GetComponent<TileAge>())
			.OrderBy(x => GetNearestDecay(x))
			.ToList();
		
		if (limit > 0) {
			for (int i=0; i<limit && i<toFix.Count; i++) {
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

		a.Repair();
		TileTracker tracker = targetTile.GetTracker();
		string msg = $"{sourceTile.name} at {tracker.PosToStr(sourceTile.boardPosition)}";
		msg += $" repaired {targetTile.name} at {tracker.PosToStr(targetTile.boardPosition)}";
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
