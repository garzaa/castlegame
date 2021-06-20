using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Clockwork/Target Networked")]
public class ClockworkTargetNetworked : ClockworkTarget {
	#pragma warning disable 0649
	[SerializeField]
	int maxDepth = 4;

	[SerializeField]
	List<TileType> networkFilters;
	#pragma warning restore 0649
	
	override public List<GameTile> GetTargets(Vector3Int position, TileTracker tracker) {
		// algorithm: 
		// getfilteredneighbor:
		// if depth > maxdepth return
		// for every neighbor:
		// if neighbor in visited return
		// getfilteredneighbor: depth+1, results, visited
		List<GameTile> targets = new List<GameTile>();
		HashSet<GameTile> visited = new HashSet<GameTile>();
		GetFilteredNeighbors(0, position, targets, visited, tracker);
		// run this filter once at the end for SPEED
		return targets
			.Where(tile => tile.IsTileType(tileFilter))
			.ToList();
	}

	// this function has 5 arguments because scriptable objects have to be stateless
	void GetFilteredNeighbors(int depth, Vector3Int position, List<GameTile> targets, HashSet<GameTile> visited, TileTracker tracker) {
		if (depth > maxDepth) return;
		depth++;

		List<GameTile> networkedTargets = tracker.GetNeighbors(position)
			.Where(tile => !visited.Contains(tile))
			.Where(tile => (tile.IsTileType(tileFilter) || TileIsInNetwork(tile)))
			.ToList();
		
		visited.UnionWith(networkedTargets);
		foreach (GameTile tile in networkedTargets) {
			GetFilteredNeighbors(depth, tile.position, targets, visited, tracker);
		}
	}

	bool TileIsInNetwork(GameTile g) {
		foreach (TileType t in networkFilters) {
			if (g.IsTileType(t)) {
				return true;
			}
		}
		return false;
	}
}
