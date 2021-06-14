using UnityEngine;

struct TilePlacement {
	public Vector3Int position;
	public ScriptableTile newTile;

	public TilePlacement(Vector3Int pos, ScriptableTile tile) {
		this.position = pos;
		this.newTile = tile;
	}
}
