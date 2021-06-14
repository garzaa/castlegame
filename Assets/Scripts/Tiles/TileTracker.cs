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
	Queue<TilePlacement> placements = new Queue<TilePlacement>();
	GameObject tileContainer;

	public static readonly string letters = "abcdefghijklmnopqrstuvwxyz";

	void Start() {
		// TODO: might need to improve this find logic
		tilemap = GameObject.FindObjectOfType<Tilemap>();
		origin = tilemap.cellBounds.min;
		tileContainer = Instantiate(new GameObject(), this.transform);

		for (int x=0; x<tilemap.cellBounds.size.x; x++) {
			List<GameTile> xRow = new List<GameTile>();
			for (int y=0; y<tilemap.cellBounds.size.y; y++) {
				Vector3Int currentPos = new Vector3Int(x, y, 0);
				ScriptableTile tile = tilemap.GetTile<ScriptableTile>(origin+currentPos);
				if (tile == null) {
					CommandInput.Log($"Null tile at ({x},{y})!");
					// preserve array shape
					xRow.Add(null);
					continue;
				}
				
				GameTile gameTile = SpawnTile(tile, currentPos);
				xRow.Add(gameTile);
			}
			tiles.Add(xRow);
		}
	}

	public GameTile GetTile(int x, int y) {
		try {
			return tiles[x][y].GetComponent<GameTile>();
		} catch (ArgumentOutOfRangeException) {
			return null;
		}
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
		// check if it's valid
		if (!ValidPlacement(tilemap.GetTile(origin + position) as ScriptableTile, newTile, position)) {
			return;
		}

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

	bool ValidPlacement(ScriptableTile oldTile, ScriptableTile newTile, Vector3Int pos) {
		TileCriterion[] criteria = newTile.tileObject.GetComponents<TileCriterion>();
		for (int i=0; i<criteria.Length; i++) {
			if (!criteria[i].Valid(this, pos)) {
				return false;
			}
		}
		return true;
	}

	public void RepairTile(Vector3Int pos) {
		GameTile tile = tiles[pos.x][pos.y];
		if (tile.GetComponent<TileAge>()) {
			tile.GetComponent<TileAge>().Repair();
			CommandInput.Log("Repaired "+tile.name+" at "+PosToStr(pos));
		} else {
			CommandInput.Log("Can't fix tile "+tile.name+" at "+PosToStr(pos));
		}
	}

	public Vector3Int StrToPos(string coords, bool validate=true) {
		int x = 0, y = 0;
		try {
			int idx = letters.IndexOf(coords[0]);
			x = int.Parse(idx.ToString());
			y = int.Parse(coords[1].ToString())-1;
		} catch (Exception) {
			CommandInput.Log("Invalid coordinates "+coords);
		}
		Vector3Int pos = new Vector3Int(x, y, 0);
		if (validate) {
			if (pos.x > tilemap.cellBounds.size.x 
			|| pos.y > tilemap.cellBounds.size.y
			|| pos.x < 0 || pos.y < 0) {
				Debug.Log(pos);
				Debug.Log(origin);
				Debug.Log(tilemap.cellBounds.size);
				CommandInput.Log("Invalid coordinates "+coords);
				throw new IndexOutOfRangeException("Invalid tilemap coordinates "+pos);
			}
			
		}
		return pos;
	}

	public string PosToStr(Vector3Int pos) {
		return letters[pos.x].ToString().ToUpper() + (pos.y + 1);
	}

	public void QueueReplacement(Vector3Int position, ScriptableTile newTile) {
		placements.Enqueue(new TilePlacement(position, newTile));
	}

	void ApplyPlacement(TilePlacement placement) {
		ReplaceTile(placement.position, placement.newTile);
	}

	public void FinishTick() {
		while (placements.Count > 0) {
			ApplyPlacement(placements.Dequeue());
		}

		placements.Clear();
	}
}
