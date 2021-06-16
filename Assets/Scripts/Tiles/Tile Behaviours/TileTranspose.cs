using UnityEngine;

public class TileTranspose : TileRedirection {
	public Vector3Int fromRelative;
	public Vector3Int toRelative;

	void OnPlace() {
		TileTracker t = gameTile.GetTracker();
		GameTile sourceTile = t.GetTileNoRedirect(fromRelative.x, fromRelative.y);
		GameTile targetTile = t.GetTileNoRedirect(fromRelative.x, fromRelative.y);
		gameTile.GetTracker().AddRedirect(new TileRedirect(sourceTile, targetTile));
	}
}

// redirect: tile -> tile
// identity: calling tile -> calling tile
// getTile -> getTile(position, fromTile)
// if in reflects, return it, sure
