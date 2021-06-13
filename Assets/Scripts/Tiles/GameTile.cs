using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class GameTile : MonoBehaviour {
	TileTracker tileTracker;
	Vector3Int position;
	ScriptableTile tile;

	// neighbors will have to be updated every time the tilemap is updated
	// how do we do this without a recursive loop
	// if tile.neighbors.doesn't contain this then call updateNeighbors
	// that'll cascade it across the entire tilemap
	// or jjust get it on every call, it's not that bad
	List<GameTile> neighbors = new List<GameTile>();

	virtual public void Initialize(TileTracker tileTracker, Vector3Int position, ScriptableTile tile) {
		this.tileTracker = tileTracker;
		this.position = position;
		this.tile = tile;
	}

	public void UpdateNeighbors() {
		neighbors.Clear();
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.up));
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.down));
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.right));
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.left));
	}

	public List<GameTile> GetNeighbors() {
		return this.neighbors;
	}

	public void Clockwork() {
		if (GetComponent<TileAge>() != null) {
			GetComponent<TileAge>().Clockwork();
		}
		if (GetComponent<TileDecay>() != null) {
			GetComponent<TileDecay>().Clockwork();
		}
	}

	public void ReplaceSelf(ScriptableTile newTile) {
		tileTracker.ReplaceTile(this.position, newTile);
	}
}
