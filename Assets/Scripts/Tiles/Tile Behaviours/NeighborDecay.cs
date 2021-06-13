using UnityEngine;
using System.Collections.Generic;

public class NeighborDecay : TileDecay, IStat {
	public ScriptableTile neighbor;

	override protected void Start() {
		base.Start();
		multiplier = GetMultiplier();
	}

	public override void Clockwork() {
		multiplier = GetMultiplier();
		base.Clockwork();
	}

	int GetMultiplier() {
		int m = 0;
		List<GameTile> neighbors = gameTile.GetNeighbors();
		for (int i=0; i<neighbors.Count; i++) {
			if (neighbors[i].GetTile().name == neighbor.name) {
				m += 1;
			}
		}
		return m;
	}

	override public string Stat() {
		return $"{neighbor.tileObject.name} neighbors: {GetMultiplier()} -> {base.Stat()}";
	}
}
