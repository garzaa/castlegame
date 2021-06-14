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
			.OrderBy(x => x.GetComponent<TileAge>().GetAge())
			.ThenByDescending(x => GetAverageDecay(x))
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
		string msg = $"{sourceTile.name} at {tracker.PosToStr(sourceTile.GetPosition())}";
		msg += $" repaired {targetTile.name} at {tracker.PosToStr(targetTile.GetPosition())}";
		CommandInput.Log(msg);
	}

	float GetAverageDecay(GameTile tile) {
		int decay = 0;
		int counter = 0;

		foreach (TileDecay d in tile.GetComponents<TileDecay>()) {
			counter++;
			decay += d.GetDecay();
		}
		
		if (counter == 0) return 0;

		return (float) decay / (float) counter;
	}
}

/**
	TODO: multiple repair reconciling algorithm
	1. have a dict of <gameTile, List<GameTile>> repairs
	2. while there are tiles that can only do one repair, do them and take them out of possible repairs
	3. if there are tiles left with multiple, look for commonalities? just do em anyway?


*/
