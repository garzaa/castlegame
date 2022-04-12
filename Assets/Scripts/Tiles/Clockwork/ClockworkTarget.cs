using UnityEngine;
using System.Collections.Generic;

public abstract class ClockworkTarget : ScriptableObject {

	#pragma warning disable 0649
	[SerializeField] 
	[Tooltip("Only target tiles of this type")]
	protected TileType tileFilter;
	#pragma warning restore 0649

	public abstract List<GameTile> GetTargets(Vector3Int position, TileTracker tracker);

	public virtual string GetTargetInfo(Vector3Int position, TileTracker tracker) {
		int t = GetTargets(position, tracker).Count;
		return $"Targeting {t} tile{(t==1 ? "" : "s")}.";
	}
}
