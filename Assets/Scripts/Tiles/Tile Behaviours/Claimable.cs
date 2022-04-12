using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class Claimable : TileBehaviour, IStat {
	public GameTile claimed {get; private set;}

	#pragma warning disable 0649
	[SerializeField] GameObject claimedEffectTemplate;
	#pragma warning restore 0649

	GameObject effect;
	bool placed = false;

	public void Claim(GameTile tile) {
		claimed = tile;
		// can be claimed by a tile earlier in the initialization order
		// so it won't have its position and everything set up
		if (placed) {
			SpawnClaimedEffect();
		}
	}

	public void UnClaim(GameTile tile) {
		GameObject.Destroy(effect);
		claimed = null;
	}
	
	public string Stat() {
		if (!claimed) return "Not claimed.";
		else return "Claimed by "+claimed+".";
	}

	public override void OnRemove(bool fromPlayer) {
		if (effect != null) {
			GameObject.Destroy(effect);
		}
	}

	public override void OnPlace() {
		if (claimed) {
			SpawnClaimedEffect();
		}
		placed = true;
	}

	void SpawnClaimedEffect() {
		effect = Instantiate(claimedEffectTemplate, gameTile.worldPosition, Quaternion.identity, null) as GameObject;
	}
}
