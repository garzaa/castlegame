using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Clockwork/Neighbor Target")]
public class ClockworkTargetNeighbors : ClockworkTarget {
	override public List<GameTile> GetTargets(Vector3Int position, TileTracker tracker) {
		return tracker.GetNeighbors(position);
	} 
}
