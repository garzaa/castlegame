using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameTile : MonoBehaviour, IStat, ICardStat, IConsoleStat, ITileHighlighter {
	ScriptableTile tile;
	TileTracker tileTracker;
	Tile warpTargetIcon;
	public Vector3Int boardPosition { get; private set; }
	public Vector3Int gridPosition { 
		get { return tileTracker.BoardToCell(boardPosition); }
	}
	public Vector3 worldPosition {
		get { return tileTracker.BoardToWorld(boardPosition); }
	}

	#pragma warning disable 0649
	[SerializeField] TileType tileType;
	[SerializeField] AudioResource onPlace;
	[SerializeField] AudioResource onQuery;
	[SerializeField] AudioResource onDestroy;
	[SerializeField] ScriptableTile defaultTile;
	#pragma warning restore 0649

	[TextArea] public string description;
	static readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	public TilemapVisuals visuals {get; private set;}

	public virtual void Initialize(TileTracker tileTracker, ScriptableTile tile, Vector3Int position, bool silent=false) {
		this.tile = tile;
		this.tileTracker = tileTracker;
		this.boardPosition = position;
		visuals = GameObject.FindObjectOfType<TilemapVisuals>();
		foreach (TileBehaviour t in GetComponents<TileBehaviour>()) {
			t.OnPlace();
		}
		if (!silent && onPlace != null) {
			onPlace.PlayFrom(tileTracker.gameObject);
		}
	}

	public void Remove(bool fromPlayer, bool silent=false) {
		if (!silent && onDestroy) {
			onDestroy.PlayFrom(tileTracker.gameObject);
		}
		foreach (TileBehaviour t in GetComponents<TileBehaviour>()) {
			t.OnRemove(fromPlayer);
		}
	}

	public void QueueForReplace(ScriptableTile newTile) {
		CommandInput.Log($"{gameObject.name} at {tileTracker.PosToStr(this.boardPosition)} turned into {newTile.tileObject.name}");
		tileTracker.QueueReplacement(this.boardPosition, newTile);
	}

	public ScriptableTile GetDefaultTile() {
		if (!tile) return defaultTile;
		return tile;
	}

	public string Stat() {
		string stat = description;
		// tiletracker won't be called if the prefab is referenced
		if (tileTracker) {
			List<Tuple<GameTile, TileWarpType>> warps = tileTracker.GetWarps(this.boardPosition);
			foreach (var warp in warps) {
				if (warp.Item1 == null) continue; // OOB - should validate this earlier
				if (!warp.Item2.Equals(TileWarpType.REFLECT)) {
					stat += $"\n<color='#ca52c9'>{TileWarp.WarpToString(warp.Item2)} to "+warp.Item1.ToString()+".</color>";
				}
			}
		}
		return stat;
	}

	public void PlayQuerySound() {
		if (onQuery) {
			onQuery.PlayFrom(tileTracker.gameObject);
		}
	}

	public List<GameTile> GetNeighbors() {
		return tileTracker.GetNeighbors(boardPosition);
	}

	public TileTracker GetTracker() {
		return tileTracker;
	}

	public override string ToString() {
		return $"{this.name} at {tileTracker.PosToStr(this.boardPosition)}";
	}

	public bool IsTileType(TileType t) {
		return this.tileType.IsType(t);
	}

	public TileType GetTileType() {
		return this.tileType;
	}

	public IStat[] GetStats() {
		return StatOrder.OrderStats(GetComponents<IStat>());
	}

	public ICardStat[] GetCardStats() {
		return StatOrder.OrderCardStats(GetComponents<ICardStat>());
	}

	public TileHighlight GetHighlight(TileTracker tracker, Vector3Int boardPosition) {
		if (!tracker.HasWarp(boardPosition)) {
			return null;
		}

		if (warpTargetIcon == null) {
			warpTargetIcon = TileWarp.LoadWarpTile("WarpTargetTile");
		}


		List<Vector3Int> targets = new List<Vector3Int>();
		List<Tuple<GameTile, TileWarpType>> warps = tracker.GetWarps(boardPosition);
		foreach (var t in warps) {
			if (!t.Item2.Equals(TileWarpType.REFLECT)) {
				// second time doing this, this isn't great
				targets.Add(t.Item1.gridPosition + Vector3Int.down + Vector3Int.left);
			}
		}

		return new TileHighlight(warpTargetIcon, targets);
	}
}
