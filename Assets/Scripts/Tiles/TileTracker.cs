using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileTracker : MonoBehaviour {
	[SerializeField] CommandInput console;

	Vector3Int origin;
	Tilemap tilemap;
	List<List<GameTile>> tiles = new List<List<GameTile>>();
	Queue<TilePlacement> placements = new Queue<TilePlacement>();
	Dictionary<ExclusiveClockworkAction, List<ClockworkApply>> exclusiveActions = new Dictionary<ExclusiveClockworkAction, List<ClockworkApply>>();
	Dictionary<GameTile, GameTile> redirects = new Dictionary<GameTile, GameTile>();
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
				
				GameTile gameTile = SpawnGameTile(tile, currentPos, initialize:false);
				xRow.Add(gameTile);
			}
			tiles.Add(xRow);
		}

		InitializeAllTiles();
	}

	void InitializeAllTiles() {
		for (int x=0; x<tiles.Count; x++) {
			for (int y=0; y<tiles.Count; y++) {
				Vector3Int currentPos = new Vector3Int(x, y, 0);
				tiles[x][y].Initialize(this, currentPos, tilemap.GetTile<ScriptableTile>(origin+currentPos));
			}
		}
	}

	public GameTile GetTileNoRedirect(Vector3Int pos) {
		return GetTileNoRedirect(pos.x, pos.y);
	}

	public GameTile GetTileNoRedirect(int x, int y) {
		try {
			return tiles[x][y].GetComponent<GameTile>();
		} catch (ArgumentOutOfRangeException) {
			return null;
		}
	}

	public GameTile GetTile(int x, int y, GameTile from) {
		try {
			GameTile tile = tiles[x][y].GetComponent<GameTile>();
			return FollowRedirects(tile, from, new List<GameTile>());
		} catch (ArgumentOutOfRangeException) {
			return null;
		}
	}

	public GameTile GetTile(Vector3Int pos, GameTile from) {
		return GetTile(pos.x, pos.y, from);
	}

	public GameTile FollowRedirects(GameTile currentTile, GameTile from, List<GameTile> visited) {
		if (redirects.ContainsKey(currentTile) && !visited.Contains(redirects[currentTile])) {
			// redirection to null == outside bounds or reflector
			if (redirects[currentTile]==null) {
				return from;
			}
			visited.Add(currentTile);
			return FollowRedirects(redirects[currentTile], from, visited);
		}
		return currentTile;
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
		return tileContainer.GetComponentsInChildren<T>(includeInactive: false);
	}

	public bool ReplaceTile(Vector3Int position, ScriptableTile newTile) {
		// check if it's valid
		if (!ValidPlacement(tilemap.GetTile(origin + position) as ScriptableTile, newTile, position)) {
			return false;
		}

		RemoveTile(position);

		tilemap.SetTile(position+origin, newTile);
		GameTile tileBackend = SpawnGameTile(newTile, position);
		tiles[position.x][position.y] = tileBackend;

		return true;
	}

	void RemoveTile(Vector3Int position) {
		tilemap.SetTile(origin+position, null);
		GameTile old = tiles[position.x][position.y];
		old.Remove();
		old.gameObject.SetActive(false);
		GameObject.Destroy(old.gameObject);
	}

	GameTile SpawnGameTile(ScriptableTile tile, Vector3Int position, bool initialize = true) {
		GameTile tileBackend = Instantiate(tile.tileObject, tileContainer.transform).GetComponent<GameTile>();
		tileBackend.gameObject.name = tile.tileObject.name;
		if (initialize) tileBackend.Initialize(this, position, tile);
		return tileBackend;
	}

	bool ValidPlacement(ScriptableTile oldTile, ScriptableTile newTile, Vector3Int pos) {
		ITileValidator[] criteria = newTile.tileObject.GetComponents<ITileValidator>();
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

	public void Tick() {
		foreach (TileAge tileAge in GetTiles<TileAge>()) {
			tileAge.Clockwork();
		}

		foreach (Clockwork clockwork in GetTiles<Clockwork>()) {
			clockwork.Tick();
		}

		ReconcileExclusiveActions();

		foreach (TileDecay decay in GetTiles<TileDecay>()) {
			decay.Clockwork();
		}

		while (placements.Count > 0) {
			ApplyPlacement(placements.Dequeue());
		}
		placements.Clear();
	}

	public List<GameTile> GetNeighbors(Vector3Int position) {
		List<GameTile> neighbors = new List<GameTile>();
		neighbors.Add(GetTile(position + Vector3Int.up, GetTileNoRedirect(position.x, position.y)));
		neighbors.Add(GetTile(position + Vector3Int.down, GetTileNoRedirect(position.x, position.y)));
		neighbors.Add(GetTile(position + Vector3Int.right, GetTileNoRedirect(position.x, position.y)));
		neighbors.Add(GetTile(position + Vector3Int.left, GetTileNoRedirect(position.x, position.y)));
		neighbors.RemoveAll(x => x==null);
		return neighbors;
	}

	public void QueueExclusiveAction(ExclusiveClockworkAction actionType, ClockworkApply spec) {
		if (!exclusiveActions.ContainsKey(actionType)) {
			exclusiveActions[actionType] = new List<ClockworkApply>();
		}
		exclusiveActions[actionType].Add(spec);
	}

	void ReconcileExclusiveActions() {
		foreach (var action in exclusiveActions) {
			ReconcileExclusiveAction(action.Key, action.Value);
		}
		exclusiveActions.Clear();
	}

	void ReconcileExclusiveAction(ExclusiveClockworkAction actionType, List<ClockworkApply> actions) {
		List<ClockworkApply> singularActions;

		// do this instead of iterating since actions will be modified each loop
		while ((singularActions = GetSingularActions(actions)).Count > 0) {
			ClockworkApply currentAction = singularActions[0];
			GameTile sourceTile = currentAction.sourceTile;
			
			// apply the action to that tile
			actionType.ExecuteApply(currentAction);

			// then remove all references to that tile from the list of actions
			PruneTargetsFromActions(currentAction.targets, actions);
		}

		// this has to be here
		actions.RemoveAll(x => x.targets.Count == 0);

		// after this, there could just be multiple actions
		// so do the same thing
		while (actions.Count > 0) {
			actionType.ExecuteApply(actions[0]);
			PruneTargetsFromActions(actions[0].targets, actions);
		}
	}

	void PruneTargetsFromActions(List<GameTile> targets, List<ClockworkApply> actions) {
		foreach (ClockworkApply action in actions) {
			action.targets.RemoveAll(x => targets.Contains(x));
		}
		actions.RemoveAll(x => x.targets.Count == 0);
	}

	List<ClockworkApply> GetSingularActions(List<ClockworkApply> actions) {
		return actions.Where(x => x.targets.Count == 1).ToList();
	}

	public void AddRedirect(TileRedirect redirect) {
		// target null means reflect, origin null means out of bounds
		if (redirect.origin == null) return;
		redirects[redirect.origin] = redirect.target;
	}

	public void RemoveRedirect(TileRedirect redirect) {
		redirects.Remove(redirect.origin);
	}

	public bool HasRedirect(int x, int y) {
		GameTile tile = GetTileNoRedirect(x, y);
		if (!tile) return false;
		return redirects.ContainsKey(tile);
	}

	public bool HasRedirect(GameTile tile) {
		if (!tile) return false;
		return redirects.ContainsKey(tile);
	}

	public GameTile GetRedirect(GameTile tile) {
		return redirects[tile];
	}
}
