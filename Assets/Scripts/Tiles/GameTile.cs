using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class GameTile : MonoBehaviour, IStat {
	TileTracker tileTracker;
	public Vector3Int position {get; private set; }
	ScriptableTile tile;
	string OnPlace;
	string AfterPlace;
	string OnRemove;

	[TextArea] public string description;
	
	static readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	public virtual void Initialize(TileTracker tileTracker, Vector3Int position, ScriptableTile tile) {
		this.tileTracker = tileTracker;
		this.position = position;
		this.tile = tile;
		SendMessage(nameof(OnPlace), SendMessageOptions.DontRequireReceiver);
	}

	public void Remove() {
		SendMessage(nameof(OnRemove), SendMessageOptions.DontRequireReceiver);
	}

	public void ReplaceSelf(ScriptableTile newTile) {
		CommandInput.Log($"{gameObject.name} at {tileTracker.PosToStr(this.position)} turned into {newTile.tileObject.name}");
		tileTracker.ReplaceTile(this.position, newTile);
	}

	public void QueueForReplace(ScriptableTile newTile) {
		CommandInput.Log($"{gameObject.name} at {tileTracker.PosToStr(this.position)} is turning into {newTile.tileObject.name}");
		tileTracker.QueueReplacement(this.position, newTile);
	}

	public ScriptableTile GetTile() {
		return tile;
	}

	public string Stat() {
		string stat = $"{name} at {tileTracker.PosToStr(this.position)}\n{description}";
		if (tileTracker.HasRedirect(this)) {
			string target = tileTracker.GetRedirect(this) == null ? "watcher" : tileTracker.GetRedirect(this).ToString();
			stat += "\nRedirects to "+target;
		}
		return stat;
	}

	public List<GameTile> GetNeighbors() {
		return tileTracker.GetNeighbors(position);
	}

	public TileTracker GetTracker() {
		return tileTracker;
	}

	public override string ToString() {
		return $"{tileTracker.PosToStr(this.position)} at {this.name}";
	}
}
