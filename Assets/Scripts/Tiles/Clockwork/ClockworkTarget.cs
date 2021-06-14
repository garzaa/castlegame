using UnityEngine;
using System.Collections.Generic;

public class ClockworkTarget : ScriptableObject {
	public virtual List<GameTile> GetTargets(Vector3Int position, TileTracker tracker) {
		return null;
	}	
}
