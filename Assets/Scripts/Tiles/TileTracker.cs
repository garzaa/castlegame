using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class TileTracker : MonoBehaviour {
	[SerializeField] CommandInput console;

	Vector3Int origin;
	Tilemap tilemap;
	List<List<GameTile>> tiles = new List<List<GameTile>>();
	GameObject tileContainer;

	public static readonly string letters = "abcdefghijklmnopqrstuvwxyz";

	void Start() {
		// TODO: might need to improve this
		tilemap = GameObject.FindObjectOfType<Tilemap>();
		origin = tilemap.cellBounds.min;
		tileContainer = Instantiate(new GameObject(), this.transform);


		for (int x=0; x<tilemap.cellBounds.size.x; x++) {
			List<GameTile> xRow = new List<GameTile>();
			for (int y=0; y<tilemap.cellBounds.size.y; y++) {
				Vector3Int normalized = origin;
				normalized.x += x;
				normalized.y += y;

				ScriptableTile tile = tilemap.GetTile<ScriptableTile>(normalized);
				if (tile == null) {
					console.Log($"null tile at ({x},{y})");
					// preserve array shape
					xRow.Add(null);
					continue;
				}
				
				GameTile gameTile = SpawnTile(tile, normalized - origin);
				xRow.Add(gameTile);
			}
			tiles.Add(xRow);
		}
	}

	public GameTile GetTile(int x, int y) {
		return tiles[x][y].GetComponent<GameTile>();
	}

	public GameTile GetTile(Vector3Int pos) {
		return GetTile(pos.x, pos.y);
	}

	public TileBase GetTilemapTile(int x, int y) {
		Vector3Int v = origin;
		v.x += x;
		v.y += y;
		return tilemap.GetTile<TileBase>(v);
	}

	public List<GameTile> GetFlatTiles() {
		return tiles.SelectMany(x => x).ToList();
	}

	public T[] GetTiles<T>() {
		return tileContainer.GetComponentsInChildren<T>();
	}

	public void ReplaceTile(Vector3Int position, ScriptableTile newTile) {
		// update the tilemap, then the internal data to reflect it
		tilemap.SetTile(position+origin, newTile);
		// then add/initialize it in the internal data structure
		GameTile tileBackend = SpawnTile(newTile, position);
		GameObject.Destroy(tiles[position.x][position.y].gameObject);
		tiles[position.x][position.y] = tileBackend;
	}

	GameTile SpawnTile(ScriptableTile tile, Vector3Int position) {
		GameTile tileBackend = Instantiate(tile.tileObject, tileContainer.transform).GetComponent<GameTile>();
		tileBackend.gameObject.name = tile.tileObject.name;
		tileBackend.Initialize(this, position, tile);
		return tileBackend;
	}

	public Vector3Int StrToPos(string coords) {
		int idx = letters.IndexOf(coords[0]);
		int x = int.Parse(idx.ToString());
		int y = int.Parse(coords[1].ToString());
		return new Vector3Int(x, y, 0);
	}
}
