using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Game Tile")]
public class GameTile : ScriptableObject {
	Tilemap tilemap;
	Vector3Int position;

	// neighbors will have to be updated every time the tilemap is updated
	// how do we do this without a recursive loop
	// if tile.neighbors.doesn't contain this then call updateNeighbors
	// that'll cascade it across the entire tilemap
	// or jjust get it on every call, it's not that bad
	List<GameTile> neighbors = new List<GameTile>();

	virtual public void Initialize(Tilemap tilemap, Vector3Int position) {
		this.tilemap = tilemap;
		this.position = position;

		UpdateNeighbors();
	}

	public void UpdateNeighbors() {
		neighbors.Clear();
		// neighbors.Add(tilemap.GetTile(position + Vector3Int.up));
		// neighbors.Add(tilemap.GetTile(position + Vector3Int.down));
		// neighbors.Add(tilemap.GetTile(position + Vector3Int.right));
		// neighbors.Add(tilemap.GetTile(position + Vector3Int.left));
	}

	public List<GameTile> GetNeighbors() {
		return this.neighbors;
	}

	virtual public void OnPlace() {

	}

	virtual public void Clockwork() {
		
	}
}
