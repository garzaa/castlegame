using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileTracker : MonoBehaviour {
	[SerializeField] CommandInput console;

	Vector3Int origin;
	Tilemap tilemap;
	List<List<GameTile>> tiles = new List<List<GameTile>>();

	/**
		on init, iterate through all the tiles
		create a new backing class of the tile

		todo: it might actually be better to make each backing type a GameObject
		because Age things and shit fits the entity-compoennt system perfectly...hough
		
	*/
	void Start() {
		tilemap = GameObject.FindObjectOfType<Tilemap>();
		origin = tilemap.cellBounds.min;
		for (int x=0; x<tilemap.cellBounds.size.x; x++) {
			List<GameTile> xRow = new List<GameTile>();
			for (int y=0; y<tilemap.cellBounds.size.y; y++) {
				Vector3Int normalized = origin;
				normalized.x += x;
				normalized.y += y;

				ScriptableTile tile = tilemap.GetTile<ScriptableTile>(normalized);
				if (tile == null) {
					console.Log($"null tile at ({x},{y})");
					continue;
				}
				
				GameTile tileBackend = Instantiate(tile.backingScript);
				xRow.Add(tileBackend);
			}
			tiles.Add(xRow);
		}
	}

	public GameTile GetTile(int x, int y) {
		return tiles[x][y];
	}

	public TileBase GetTilemapTile(int x, int y) {
		Vector3Int v = origin;
		v.x += x;
		v.y += y;
		return tilemap.GetTile<TileBase>(v);
	}

}
