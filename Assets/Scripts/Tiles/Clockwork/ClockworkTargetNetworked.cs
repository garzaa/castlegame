using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Clockwork/Target Networked")]
public class ClockworkTargetNetworked : ClockworkTarget {
	#pragma warning disable 0649
	[SerializeField]
	protected int maxDepth = 4;

	[SerializeField]
	[Tooltip("Branch through these tile types")]
	List<TileType> networkFilters;
	#pragma warning restore 0649
	
	override public List<GameTile> GetTargets(Vector3Int boardPosition, TileTracker tracker) {
		HashSet<GameTile> targets = new HashSet<GameTile>();
		Dictionary<GameTile, int> visitedWithDepths = new Dictionary<GameTile, int>();
		GetFilteredNeighbors(0, boardPosition, targets, visitedWithDepths, tracker);
		// run this filter once at the end for SPEED
		return targets
			.Where(tile => IsTargetable(tile))
			.ToList();
	}

	override public List<GameTile> GetTargetsWithVisited(Vector3Int boardPosition, TileTracker tracker) {
		HashSet<GameTile> targets = new HashSet<GameTile>();
		Dictionary<GameTile, int> visitedWithDepths = new Dictionary<GameTile, int>();
		GetFilteredNeighbors(0, boardPosition, targets, visitedWithDepths, tracker);
		targets.UnionWith(visitedWithDepths.Keys);
		// run this filter once at the end for SPEED
		return targets
			// .Where(tile => IsTargetable(tile))
			.ToList();
	}

	// this function has 5 arguments because scriptable objects have to be stateless
	void GetFilteredNeighbors(int depth, Vector3Int position, HashSet<GameTile> targets, Dictionary<GameTile, int> visitedWithDepths, TileTracker tracker) {
		if (++depth > maxDepth) {
			return;
		}

		List<GameTile> networkedTargets = tracker.GetNeighbors(position)
			// can loop back to a neighbor at a deeper depth than intended
			// so instead, map a tile to its max depth visited
			// if current depth is lower, then can still look through that node
			.Where(tile => !visitedWithDepths.ContainsKey(tile) || depth < visitedWithDepths[tile])
			.Where(tile => (IsTargetable(tile) || IsInNetwork(tile)))
			.ToList();

		// then update current depths
		foreach (GameTile networkedTarget in networkedTargets) {
			visitedWithDepths[networkedTarget] = depth;
		}

		targets.UnionWith(networkedTargets);

		foreach (GameTile tile in networkedTargets) {
			GetFilteredNeighbors(depth, tile.boardPosition, targets, visitedWithDepths, tracker);
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
