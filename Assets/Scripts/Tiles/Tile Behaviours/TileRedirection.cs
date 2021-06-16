using UnityEngine;

public class TileRedirection : TileBehaviour, ITileValidator {
	virtual public bool Valid(TileTracker tracker, Vector3Int pos) {
		bool redirected = tracker.HasRedirect(pos.x, pos.y);
		if (redirected) {
			CommandInput.Log($"Tile {tracker.GetTileNoRedirect(pos.x, pos.y).ToString()} already has a redirect on it");
			return false;
		} else {
			return true;
		}
	}
}
