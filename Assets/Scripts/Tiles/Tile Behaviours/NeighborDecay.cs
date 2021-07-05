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
			base.decayThreshold = base.originalDecayThreshold - n;
			base.decayThreshold = Mathf.Max(base.decayThreshold, 0);
			return base.GetDecay();
		} else if (decayMode == DecayMode.MUL) {
			base.decayThreshold = Mathf.CeilToInt((float) base.originalDecayThreshold / (float) n);
			base.decayThreshold = Mathf.Max(base.decayThreshold, 0);
			return base.GetDecay();
		} else {
			// this should NEVER be called
			return base.GetDecay();
		}
	}

	int GetNumNeighbors() {
		int m = 0;
		List<GameTile> neighbors = gameTile.GetNeighbors();
		for (int i=0; i<neighbors.Count; i++) {
			if (neighbors[i].GetDefaultTile().name == neighbor.name) {
				m += 1;
			}
		}
		return m;
	}

	override public string Stat() {
		if (GetNumNeighbors() == 0) return null;
		return $"{decayMode.ToString()} {neighbor.tileObject.name} neighbors: {GetNumNeighbors()} -> {base.Stat()}";
	}
}

public enum DecayMode {
	ADD = 0,
	MUL = 1
}
