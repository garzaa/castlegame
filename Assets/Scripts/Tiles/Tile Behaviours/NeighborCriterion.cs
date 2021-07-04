using UnityEngine;
using System.Collections.Generic;

public class NeighborCriterion : TileBehaviour, ITileValidator {
	public ScriptableTile neighborTile;

	public bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message) {
		foreach (GameTile tile in tracker.GetNeighbors(pos)) {
			if (tile.GetDefaultTile().name == neighborTile.name) {
				return true;
			}
		}
		string m = $"{name} needs a neighboring {neighborTile.tileObject.name}";
		message.Add(m);
		CommandInput.Log(m);
		return false;
	}
}
