using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileHighlight {
	public Dictionary<Tile, List<Vector3Int>> targetMap { get; private set; }

	public TileHighlight(Tile tile, List<Vector3Int> targets) {
		targetMap = new Dictionary<Tile, List<Vector3Int>>();
		targetMap[tile] = targets;
	}

	public TileHighlight() {
		targetMap = new Dictionary<Tile, List<Vector3Int>>();
	}

	public void SetHighlight(Tile t, List<Vector3Int> positions) {
		targetMap[t] = positions;
	}
}
