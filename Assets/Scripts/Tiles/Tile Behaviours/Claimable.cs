using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class Claimable : TileBehaviour, IStat {
	public GameTile claimed {get; private set;}

	public void Claim(GameTile tile) {
		claimed = tile;
	}

	public void UnClaim(GameTile tile) {
		claimed = null;
	}
	
	public string Stat() {
		if (!claimed) return "Not claimed.";
		else return "Claimed by "+claimed+".";
	}
}
