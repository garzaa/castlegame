using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileHighlight {
	public Tile tile { get; private set; } 
	public List<Vector3Int> targets { get; private set; }

	public TileHighlight(Tile tile, List<Vector3Int> targets) {
		this.tile = tile;
		this.targets = targets;
	}
}
