using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(menuName = "Clockwork/Fix Action")]
public class ClockworkFixAction : ExclusiveClockworkAction {
	public int limit = -1;

	public override List<GameTile> ExecuteApply(in List<GameTile> sortedTargets, in GameTile source) {
		List<GameTile> repairedTiles = new List<GameTile>();	
		if (limit > 0) {
			for (int i=0; i<limit && i<sortedTargets.Count; i++) {
				RepairTile(sortedTargets[i], source);
				repairedTiles.Add(sortedTargets[i]);
			}
		} else {
			foreach (GameTile t in sortedTargets) {
				RepairTile(t, source);
				repairedTiles.Add(t);
			}
		}

		return repairedTiles;
	}

	public override Func<GameTile, float> GetPriorityComparator() {
		return tile => tile.GetComponent<TileAge>() != null ? GetNearestDecay(tile) : int.MaxValue;
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
		int decayWindow = int.MaxValue;

		foreach (TileDecay d in tile.GetComponents<TileDecay>()) {
			if (!d.IsActive()) continue;
			decayWindow = Mathf.Min(d.GetDecayThreshold() - d.GetDecay(), decayWindow);
		}

		return decayWindow;
	}
}
