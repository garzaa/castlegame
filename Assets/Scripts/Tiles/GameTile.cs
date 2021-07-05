using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameTile : MonoBehaviour, IStat, ICardStat, IConsoleStat {
	TileTracker tileTracker;
	public Vector3Int position {get; private set; }
	public Vector3Int gridPosition { 
		get { return tileTracker.BoardToCell(position); }
	}
	public Vector3 worldPosition {
		get { return tileTracker.BoardToWorld(position); }
	}

	#pragma warning disable 0649
	[SerializeField] TileType tileType;
	[SerializeField] AudioResource onPlace;
	[SerializeField] AudioResource onQuery;
	[SerializeField] AudioResource onDestroy;
	[SerializeField] ScriptableTile tile;
	#pragma warning restore 0649

	[TextArea] public string description;
	static readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	public TilemapVisuals visuals {get; private set;}

	public virtual void Initialize(TileTracker tileTracker, Vector3Int position, bool silent=false) {
		this.tileTracker = tileTracker;
		this.position = position;
		foreach (TileBehaviour t in GetComponents<TileBehaviour>()) {
			t.OnPlace();
		}
		if (!silent && onPlace != null) {
			onPlace.PlayFrom(tileTracker.gameObject);
		}
		visuals = GameObject.FindObjectOfType<TilemapVisuals>();
	}

	public void Remove(bool silent=false) {
		if (!silent && onDestroy) {
			onDestroy.PlayFrom(tileTracker.gameObject);
		}
		foreach (TileBehaviour t in GetComponents<TileBehaviour>()) {
			t.OnRemove();
		}
	}

	public void QueueForReplace(ScriptableTile newTile) {
		CommandInput.Log($"{gameObject.name} at {tileTracker.PosToStr(this.position)} turned into {newTile.tileObject.name}");
		tileTracker.QueueReplacement(this.position, newTile);
	}

	public ScriptableTile GetDefaultTile() {
		return tile;
	}

	public string Stat() {
		string stat = description;
		// tiletracker won't be called if the prefab is referenced
		if (tileTracker) {
			List<Tuple<GameTile, TileWarpType>> warps = tileTracker.GetWarps(this.position);
			foreach (var warp in warps) {
				if (!warp.Item2.Equals(TileWarpType.REFLECT)) {
					stat += $"\n<color='#ca52c9'>{TileWarp.WarpToString(warp.Item2)} to "+warp.Item1.ToString()+".</color>";
				}
				else {
					stat += "\n<color='#ca52c9'>Redirects to watcher.</color>";
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
		return tileTracker.GetNeighbors(position);
	}

	public TileTracker GetTracker() {
		return tileTracker;
	}

	public override string ToString() {
		return $"{this.name} at {tileTracker.PosToStr(this.position)}";
	}

	public bool IsTileType(TileType t) {
		return this.tileType.IsType(t);
	}
}
