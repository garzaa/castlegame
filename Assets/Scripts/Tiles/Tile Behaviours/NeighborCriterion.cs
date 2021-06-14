using UnityEngine;

public class NeighborCriterion : TileCriterion {
	public ScriptableTile neighborTile;

	public override bool Valid(TileTracker tracker, Vector3Int pos) {
		foreach (GameTile tile in tracker.GetNeighbors(pos)) {
			if (tile.GetTile().name == neighborTile.name) {
				return true;
			}
		}
		CommandInput.Log($"{name} needs a neighboring {neighborTile.tileObject.name}");
		return false;
	}
}
