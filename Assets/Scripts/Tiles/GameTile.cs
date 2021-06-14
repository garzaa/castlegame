using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class GameTile : MonoBehaviour, IStat {
	TileTracker tileTracker;
	Vector3Int position;
	ScriptableTile tile;
	string OnPlace;
	string OnRemove;

	[TextArea] public string description;

	public void Initialize(TileTracker tileTracker, Vector3Int position, ScriptableTile tile) {
		this.tileTracker = tileTracker;
		this.position = position;
		this.tile = tile;
		SendMessage(nameof(OnPlace), SendMessageOptions.DontRequireReceiver);
	}

	public void Remove() {
		SendMessage(nameof(OnRemove), SendMessageOptions.DontRequireReceiver);
	}

	public List<GameTile> GetNeighbors() {
		List<GameTile> neighbors = new List<GameTile>();
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.up));
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.down));
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.right));
		neighbors.Add(tileTracker.GetTile(position + Vector3Int.left));
		neighbors.RemoveAll(x => x==null);
		return neighbors;
	}

	public void Clockwork() {
		if (GetComponent<TileAge>() != null) {
			GetComponent<TileAge>().Clockwork();
		}

		//TODO: put generic clockwork here

		if (GetComponent<TileDecay>() != null) {
			TileDecay[] d = GetComponents<TileDecay>();
			for (int i=0; i<d.Length; i++) {
				d[i].Clockwork();
			}
		}
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
		return $"{name} at {tileTracker.PosToStr(this.position)}\n{description}";
	}
}
