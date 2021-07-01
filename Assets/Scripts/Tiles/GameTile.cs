using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
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
	ScriptableTile tile;

	#pragma warning disable 0649
	string OnPlace;
	string OnRemove;
	[SerializeField] TileType tileType;
	[SerializeField] AudioResource onPlace;
	[SerializeField] AudioResource onQuery;
	[SerializeField] AudioResource onDestroy;
	#pragma warning restore 0649

	[TextArea] public string description;
	static readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	public virtual void Initialize(TileTracker tileTracker, Vector3Int position, ScriptableTile tile, bool silent=false) {
		this.tileTracker = tileTracker;
		this.position = position;
		this.tile = tile;
		SendMessage(nameof(OnPlace), SendMessageOptions.DontRequireReceiver);
		if (!silent && onPlace) {
			onPlace.PlayFrom(tileTracker.gameObject);
		}
	}

	public void Remove(bool silent=false) {
		if (!silent && onDestroy) {
			onDestroy.PlayFrom(tileTracker.gameObject);
		}
		SendMessage(nameof(OnRemove), SendMessageOptions.DontRequireReceiver);
	}

	public void QueueForReplace(ScriptableTile newTile) {
		CommandInput.Log($"{gameObject.name} at {tileTracker.PosToStr(this.position)} turned into {newTile.tileObject.name}");
		tileTracker.QueueReplacement(this.position, newTile);
	}

	public ScriptableTile GetTile() {
		return tile;
	}

	public string Stat() {
		string stat = description;
		if (tileTracker.HasRedirect(this)) {
			string target = tileTracker.GetRedirect(this) == null ? "watcher." : tileTracker.GetRedirect(this).ToString()+".";
			stat += "\nRedirects to "+target;
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
