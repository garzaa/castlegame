using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TileTraversal {
	Dictionary<Vector3Int, HashSet<Vector3Int>> positions = new Dictionary<Vector3Int, HashSet<Vector3Int>>();

	public void AddTarget(Vector3Int from, Vector3Int to) {
		if (!positions.ContainsKey(from)) {
			positions[from] = new HashSet<Vector3Int>();
		}

		positions[from].Add(to);
	}

	public void RemoveTarget(Vector3Int from, Vector3Int to) {
		if (positions.ContainsKey(from)) {
			positions[from].Remove(to);
			if (positions[from].Count == 0) {
				positions.Remove(from);
			}
		}
	}

	public List<Vector3Int> GetTargets(Vector3Int from) {
		return positions[from].ToList();
	}

	public bool HasTargets(Vector3Int from) {
		return positions.ContainsKey(from);
	}
}
