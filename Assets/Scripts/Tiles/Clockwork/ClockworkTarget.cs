using UnityEngine;
using System.Collections.Generic;

public class ClockworkTarget : ScriptableObject {

	#pragma warning disable 0649
	[SerializeField] protected TileType tileFilter;
	#pragma warning restore 0649

	public virtual List<GameTile> GetTargets(Vector3Int position, TileTracker tracker) {
		return null;
	}

	public virtual string GetTargetInfo(Vector3Int position, TileTracker tracker) {
		throw new System.NotImplementedException();
	}
}
