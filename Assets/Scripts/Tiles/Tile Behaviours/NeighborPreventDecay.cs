using UnityEngine;
using System.Collections.Generic;

public class NeighborPreventDecay : TileDecay, IStat, ICardStat {
	#pragma warning disable 0649
	public GameTile neighbor;
	#pragma warning restore 0649

	override public int GetDecay() {
		if (HasNeighbor()) {
			return 0;
		} else {
			return base.GetDecay();
		}
	}

	override public string Stat() {
		if (inGame && !HasNeighbor()) {
			return "Has no neighboring "+neighbor.name+": -> "+base.Stat();
		}
		return null;
	}

	bool HasNeighbor() {
		List<GameTile> neighbors = gameTile.GetTracker().GetNeighbors(gameTile.boardPosition);
		foreach (GameTile g in neighbors) {
			if (g.name == neighbor.name) {
				return true;
			}
		}
		return false;
	}
}
