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
		List<GameTile> targets = new List<GameTile>();
		HashSet<GameTile> visited = new HashSet<GameTile>();
		GetFilteredNeighbors(0, position, targets, visited, tracker);
		// run this filter once at the end for SPEED
		return targets
			.Where(tile => IsTargetable(tile))
			.ToList();
	}

	// this function has 5 arguments because scriptable objects have to be stateless
	void GetFilteredNeighbors(int depth, Vector3Int position, List<GameTile> targets, HashSet<GameTile> visited, TileTracker tracker) {
		if (++depth > maxDepth) {
			return;
		}

		List<GameTile> networkedTargets = tracker.GetNeighbors(position)
			.Where(tile => !visited.Contains(tile))
			.Where(tile => (IsTargetable(tile) || IsInNetwork(tile)))
			.ToList();

		visited.UnionWith(networkedTargets);
		targets.AddRange(networkedTargets);

		foreach (GameTile tile in networkedTargets) {
			GetFilteredNeighbors(depth, tile.boardPosition, targets, visited, tracker);
		}
	}

	public override string GetTargetInfo(Vector3Int position, TileTracker tracker) {
		return base.GetTargetInfo(position, tracker) + "\n<color=#657392>Max network depth: "+maxDepth+"</color>";
	}

	bool IsInNetwork(GameTile g) {
		foreach (TileType t in networkFilters) {
			if (g.IsTileType(t)) {
				return true;
			}
		}
		return false;
	}

	public string GetTargetType() {
		return tileFilter.name;
	}

	virtual protected bool IsTargetable(GameTile tile) {
		return tile.IsTileType(tileFilter);
	}
}
