using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Clockwork/Neighbor Target")]
public class ClockworkTargetNeighbors : ClockworkTarget {
	public bool onlyStructures;
	override public List<GameTile> GetTargets(Vector3Int position, TileTracker tracker) {
		List<GameTile> targets = tracker.GetNeighbors(position);
		if (onlyStructures) targets = targets.Where(x => x.GetComponent<StructureTile>()).ToList();
		return targets;
	} 
}
