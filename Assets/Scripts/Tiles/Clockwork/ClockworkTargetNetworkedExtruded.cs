using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(menuName="Clockwork/Target Networked Extruded")]
public class ClockworkTargetNetworkedExtruded : ClockworkTargetNetworked {
	override public List<GameTile> GetTargets(Vector3Int boardPosition, TileTracker tracker) {
		List<GameTile> targets = base.GetTargets(boardPosition, tracker);

		HashSet<GameTile> extrudedNeighbors = new HashSet<GameTile>();
		foreach (GameTile tile in targets) {
			extrudedNeighbors.UnionWith(tracker.GetNeighbors(tile.boardPosition));
		}

		extrudedNeighbors.UnionWith(targets);

		return extrudedNeighbors.ToList();
	}

}
