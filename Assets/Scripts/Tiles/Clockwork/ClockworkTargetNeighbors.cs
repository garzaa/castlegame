using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Clockwork/Neighbor Target")]
public class ClockworkTargetNeighbors : ClockworkTarget {
	override public List<GameTile> GetTargets(Vector3Int position, TileTracker tracker) {
		List<GameTile> targets = tracker.GetNeighbors(position);
		if (tileFilter != null) targets = targets.Where(x => x.GetComponent<GameTile>().IsTileType(tileFilter)).ToList();
		return targets;
	} 
}
