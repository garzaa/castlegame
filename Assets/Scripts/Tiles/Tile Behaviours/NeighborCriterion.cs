using UnityEngine;

public class NeighborCriterion : TileBehaviour, ITileValidator {
	public ScriptableTile neighborTile;

	public bool Valid(TileTracker tracker, Vector3Int pos) {
		foreach (GameTile tile in tracker.GetNeighbors(pos)) {
			Debug.Log(tile);
			if (tile.GetDefaultTile().name == neighborTile.name) {
				return true;
			}
		}
		CommandInput.Log($"{name} needs a neighboring {neighborTile.tileObject.name}");
		return false;
	}
}
