using UnityEngine;
using System;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class TileTracker : MonoBehaviour {

	#pragma warning disable 0649
	[SerializeField] GameEvent boardChangeEvent;
	#pragma warning restore 0649

	CommandInput console;
	Vector3Int origin;
	Tilemap tilemap;
	List<List<GameTile>> tiles = new List<List<GameTile>>();
	Queue<TilePlacement> placements = new Queue<TilePlacement>();
	Dictionary<Type, ExclusiveActionMatrix> exclusiveActions = new Dictionary<Type, ExclusiveActionMatrix>();
	Dictionary<TileWarpType, TileTraversal> tileWarps = new Dictionary<TileWarpType, TileTraversal>();
	TileTraversal redirects = new TileTraversal();
	TileTraversal copies = new TileTraversal();
	TileTraversal reflects = new TileTraversal();
	GameObject tileContainer;
	Vector3Int gridMousePos;

	public static readonly string letters = "abcdefghijklmnopqrstuvwxyz";

	void Awake() {
		tileWarps[TileWarpType.REDIRECT] = redirects;
		tileWarps[TileWarpType.COPY] = copies;
		tileWarps[TileWarpType.REFLECT] = reflects;
	}

	void Start() {
		console = GameObject.FindObjectOfType<CommandInput>();
		tilemap = GameObject.FindObjectOfType<MainTilemap>().GetComponentInChildren<Tilemap>();
		tilemap.CompressBounds();
		origin = tilemap.cellBounds.min;
		tileContainer = Instantiate(new GameObject(), this.transform);
		tileContainer.name = "Game Tiles";

		for (int x=0; x<tilemap.cellBounds.size.x; x++) {
			List<GameTile> xRow = new List<GameTile>();
			for (int y=0; y<tilemap.cellBounds.size.y; y++) {
				Vector3Int currentPos = new Vector3Int(x, y, 0);
				ScriptableTile tile = tilemap.GetTile<ScriptableTile>(origin+currentPos);
				if (tile == null) {
					String l = $"Null tile at ({x},{y})!";
					Debug.LogWarning(l);
					CommandInput.Log(l);
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

	public GameTile GetTileFromWorld(Vector3 worldPos) {
		return GetTileNoRedirect(CellToBoard(tilemap.WorldToCell(worldPos)));
	}

	public Vector3Int CellToBoard(Vector3Int pos) {
		return pos - origin;
	}

	public Vector3Int BoardToCell(Vector3Int pos) {
		return origin + pos;
	}

	public Vector3Int WorldToBoard(Vector3 pos) {
		return CellToBoard(tilemap.WorldToCell(pos));
	}

	public bool CellInBounds(Vector3Int cellPos) {
		bool b = (
			   cellPos.x <= tilemap.cellBounds.max.x 
			&& cellPos.y <= tilemap.cellBounds.max.y
			&& cellPos.x >= tilemap.cellBounds.min.x
			&& cellPos.y >= tilemap.cellBounds.min.y
		);
		if (!b) return false;

		return b;
	}

	public bool BoardInBounds(Vector3Int boardPos) {
		return CellInBounds(BoardToCell(boardPos));
	}

	void InitializeAllTiles() {
		for (int x=0; x<tiles.Count; x++) {
			for (int y=0; y<tiles[x].Count; y++) {
				Vector3Int currentPos = new Vector3Int(x, y, 0);
				ScriptableTile tile = GetTilemapTile(x, y) as ScriptableTile;
				tiles[x][y].Initialize(this, tile, currentPos, silent:true);
			}
		}
	}

	public GameTile GetTileNoRedirect(Vector3Int boardPos) {
		return GetTileNoRedirect(boardPos.x, boardPos.y);
	}

	public GameTile GetTileNoRedirect(int x, int y) {
		try {
			return tiles[x][y].GetComponent<GameTile>();
		} catch (ArgumentOutOfRangeException) {
			return null;
		}
	}

	public List<GameTile> GetTile(Vector3Int pos, GameTile from) {
		HashSet<GameTile> results = new HashSet<GameTile>();
		FollowRedirects(pos, from, ref results, new HashSet<Vector3Int>());
		return new List<GameTile>(results);
	}

	void FollowRedirects(Vector3Int currentPos, GameTile from, ref HashSet<GameTile> results, HashSet<Vector3Int> visited) {
		if (!HasWarp(currentPos)) {
			results.Add(GetTileNoRedirect(currentPos));
			return;
		}

		foreach (KeyValuePair<TileWarpType, TileTraversal> kv in tileWarps) {
			TileWarpType warpType = kv.Key;
			TileTraversal traversal = kv.Value;
			if (traversal.HasTargets(currentPos)) {
				foreach (Vector3Int warpTarget in traversal.GetTargets(currentPos)) {
					// reflection is a special case, it might not have immediate neighbors
					// and is treated as 
					// it can have been visited by other warps, but it always needs to hit the reflection here
					if (warpType == TileWarpType.REFLECT) {
						results.Add(from);
						visited.Add(currentPos);
						visited.Add(warpTarget);
						FollowRedirects(from.boardPosition, from, ref results, visited);
					}

					if (!visited.Contains(warpTarget)) {
						switch (warpType) {
							case TileWarpType.COPY:
								results.Add(GetTileNoRedirect(currentPos));
								goto case TileWarpType.REDIRECT;

							case TileWarpType.REDIRECT:
								visited.Add(currentPos);
								visited.Add(warpTarget);
								FollowRedirects(warpTarget, from, ref results, visited);
								break;
						}
					} else {
						results.Add(GetTileNoRedirect(currentPos));
					}
				}
			}
		}
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

	public GameTile ReplaceTile(Vector3Int boardPosition, ScriptableTile newTile, bool validate=false, bool fromPlayer=false) {
		GameTile oldTileBackend = GetTileNoRedirect(boardPosition);

		GameTile newTileBackend = SpawnGameTile(newTile, boardPosition);
		if (fromPlayer) newTileBackend.SendMessage("OnBuild", SendMessageOptions.DontRequireReceiver);
		RemoveTile(boardPosition, fromPlayer);

		tilemap.SetTile(boardPosition+origin, newTile);
		tiles[boardPosition.x][boardPosition.y] = newTileBackend;

		boardChangeEvent.Raise();
		return newTileBackend;
	}

	public void SendBoardChanged() {
		boardChangeEvent.Raise();
	}

	void RemoveTile(Vector3Int position, bool fromPlayer) {
		tilemap.SetTile(origin+position, null);
		GameTile old = tiles[position.x][position.y];
		old.Remove(fromPlayer);
		old.gameObject.SetActive(false);
		Destroy(old.gameObject);
	}

	GameTile SpawnGameTile(ScriptableTile tile, Vector3Int position, bool initialize = true) {
		GameTile tileBackend = Instantiate(tile.tileObject, tileContainer.transform).GetComponent<GameTile>();
		tileBackend.gameObject.name = tile.tileObject.name;
		if (initialize) tileBackend.Initialize(this, tile, position);
		return tileBackend;
	}

	public PlacementTestResult ValidPlacement(ScriptableTile newTile, Vector3Int pos) {
		return ValidPlacement(newTile.tileObject.GetComponent<GameTile>(), pos);
	}

	public PlacementTestResult ValidPlacement(GameTile newTile, Vector3Int pos) {
		ITileValidator[] criteria = newTile.GetComponents<ITileValidator>();
		bool valid = true;
		List<string> message = new List<string>();
		for (int i=0; i<criteria.Length; i++) {
			if (!criteria[i].Valid(this, pos, ref message)) {
				valid = false;
			}
		}
		return new PlacementTestResult(valid, String.Join("\n", message));
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
			y = int.Parse(coords.Substring(1).ToString())-1;
		} catch (Exception) {
			CommandInput.Log("Invalid coordinates "+coords);
		}
		Vector3Int pos = new Vector3Int(x, y, 0);
		if (validate) {
			if (!CellInBounds(pos)) {
				CommandInput.Log("Invalid coordinates "+coords);
				throw new IndexOutOfRangeException("Invalid tilemap coordinates "+pos);
			}
			
		}
		return pos;
	}

	public string PosToStr(Vector3Int boardPos) {
		return letters[boardPos.x].ToString().ToUpper() + (boardPos.y + 1);
	}

	public Vector3 BoardToWorld(Vector3Int tileBoardPos) {
		return tilemap.CellToWorld(origin+tileBoardPos);
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

		foreach (ITicker t in GetTiles<ITicker>()) {
			t.Tick();
		}

		foreach (ExclusiveActionMatrix matrix in exclusiveActions.Values) {
			matrix.Resolve();
		}
		exclusiveActions.Clear();

		foreach (TileDecay decay in GetTiles<TileDecay>()) {
			decay.Clockwork();
		}

		while (placements.Count > 0) {
			ApplyPlacement(placements.Dequeue());
		}
		placements.Clear();
		// pick up age changes etc
		boardChangeEvent.Raise();
	}

	public List<GameTile> GetNeighbors(Vector3Int boardPosition) {
		List<GameTile> neighbors = new List<GameTile>();
		neighbors.AddRange(GetTile(boardPosition + Vector3Int.up, GetTileNoRedirect(boardPosition.x, boardPosition.y)));
		neighbors.AddRange(GetTile(boardPosition + Vector3Int.down, GetTileNoRedirect(boardPosition.x, boardPosition.y)));
		neighbors.AddRange(GetTile(boardPosition + Vector3Int.right, GetTileNoRedirect(boardPosition.x, boardPosition.y)));
		neighbors.AddRange(GetTile(boardPosition + Vector3Int.left, GetTileNoRedirect(boardPosition.x, boardPosition.y)));
		neighbors.RemoveAll(x => x==null);
		return neighbors;
	}

	public void QueueExclusiveAction(ExclusiveClockworkAction action, ClockworkApply spec) {
		Type actionType = action.GetType();
		if (!exclusiveActions.ContainsKey(actionType)) {
			exclusiveActions[actionType] = new ExclusiveActionMatrix();
		}
		exclusiveActions[actionType].Add(spec);
	}

	public List<Tuple<GameTile, TileWarpType>> GetWarps(Vector3Int pos) {

		List<Tuple<GameTile, TileWarpType>> results = new List<Tuple<GameTile, TileWarpType>>();
		if (GetTileNoRedirect(pos) == null) return results;

		foreach (KeyValuePair<TileWarpType, TileTraversal> kv in tileWarps) {
			TileWarpType currentWarpType = kv.Key;
			TileTraversal currentWarps = kv.Value;
			if (currentWarps.HasTargets(pos)) {
				foreach (Vector3Int targetPos in currentWarps.GetTargets(pos)) {
					results.Add(new Tuple<GameTile, TileWarpType>(GetTileNoRedirect(targetPos), currentWarpType));
				}
			}
		}

		return results;
	}

	public void AddWarp(Vector3Int from, Vector3Int to, TileWarpType warpType) {
		if (!BoardInBounds(to)) return;
		tileWarps[warpType].AddTarget(from, to);
	}

	public void RemoveWarp(Vector3Int from, Vector3Int to, TileWarpType warpType) {
		tileWarps[warpType].RemoveTarget(from, to);
	}

	public bool HasWarp(Vector3Int from) {
		foreach (TileTraversal traversal in tileWarps.Values) {
			if (traversal.HasTargets(from)) {
				return true;
			}
		}

		return false;
	}

	public int ContainsTile(ScriptableTile targetTile) {
		if (!tilemap) tilemap = GameObject.FindObjectOfType<MainTilemap>().GetComponentInChildren<Tilemap>();
		int count = tilemap.GetTilesBlock(tilemap.cellBounds)
			.Where(x=> x.name == targetTile.name)
			.Count();
		return count;
	}
}
