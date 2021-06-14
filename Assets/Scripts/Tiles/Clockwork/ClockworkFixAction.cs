using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Clockwork/Fix Action")]
public class ClockworkFixAction : ClockworkAction {
	public int limit = -1;
	public bool onlyStructures;

	// TODO: prioritize lower decay thresholds if age ties
	override public void Apply(List<GameTile> tiles, GameTile from) {
		List<TileAge> toFix = tiles
			.Where(x=> x.GetComponent<TileAge>())
			.Select(x => x.GetComponent<TileAge>())
			.OrderBy(x => x.GetAge())
			.ToList();
		
		if (limit > 0) {
			for (int i=0; i<limit; i++) {
				RepairTile(toFix[i], from);
			}
		} else {
			foreach (TileAge t in toFix) {
				RepairTile(t, from);
			}
		}
	}

	void RepairTile(TileAge tile, GameTile from){
		if (tile.GetAge() < 1) return;
		if (onlyStructures && !tile.GetComponent<StructureTile>()) return;
		tile.Repair();
		TileTracker tracker = tile.gameTile.GetTracker();
		string msg = $"{from.name} at {tracker.PosToStr(from.GetPosition())}";
		msg += $" repaired {tile.name} at {tracker.PosToStr(tile.gameTile.GetPosition())}";
		CommandInput.Log(msg);
	}
}
