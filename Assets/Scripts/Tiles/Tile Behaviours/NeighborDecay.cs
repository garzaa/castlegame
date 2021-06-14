using UnityEngine;
using System.Collections.Generic;

public class NeighborDecay : TileDecay, IStat {
	public ScriptableTile neighbor;

	[SerializeField]
	DecayMode decayMode = DecayMode.MUL;

	override protected void Start() {
		base.Start();
	}

	public override int GetDecay() {
		int n = GetNumNeighbors();
		if (n == 0) return 0;
		if (decayMode == DecayMode.ADD) {
			return base.GetDecay() + n;
		} else if (decayMode == DecayMode.MUL) {
			return base.GetDecay() * n;
		} else {
			// this should NEVER be called
			return base.GetDecay();
		}
	}

	int GetNumNeighbors() {
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
		return $"{neighbor.tileObject.name} neighbors: {GetNumNeighbors()} -> {base.Stat()}";
	}
}

public enum DecayMode {
	ADD = 0,
	MUL = 1
}
