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
				
				GameTile gameTile = SpawnGameTile(tile, currentPos);
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

	GameTile SpawnGameTile(ScriptableTile tile, Vector3Int position) {
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
		neighbors.Add(GetTile(position + Vector3Int.up));
		neighbors.Add(GetTile(position + Vector3Int.down));
		neighbors.Add(GetTile(position + Vector3Int.right));
		neighbors.Add(GetTile(position + Vector3Int.left));
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

		// after this, there could just be multiple actions
		// so do the same thing
		while (actions.Count > 0) {
			actionType.ExecuteApply(actions[0]);
			PruneTargetsFromActions(actions[0].targets, actions);
		}
	}

	void PruneTargetsFromActions(List<GameTile> targets, List<ClockworkApply> actions) {
		List<ClockworkApply> toRemove = new List<ClockworkApply>();

		// if the action contains the target, remove it
		foreach (ClockworkApply currentAction in actions) {
			List<GameTile> removeTargets = new List<GameTile>();
			foreach (GameTile target in targets) {
				if (currentAction.targets.Contains(target)) {
					removeTargets.Add(target);
				}
			}
			foreach(GameTile target in removeTargets) {
				currentAction.targets.Remove(target);
			}

			// if no targets left in the action, remove it from the list of actions
			if (currentAction.targets.Count == 0) {
				toRemove.Add(currentAction);
			}
		}
		foreach (ClockworkApply emptyAction in toRemove) {
			actions.Remove(emptyAction);
		}
	}

	List<ClockworkApply> GetSingularActions(List<ClockworkApply> actions) {
		return actions.Where(x => x.targets.Count == 1).ToList();
	}
}
