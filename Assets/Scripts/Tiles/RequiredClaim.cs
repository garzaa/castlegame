using UnityEngine;
using System.Linq;

public class RequiredClaim : TileBehaviour, ITileValidator {
	[Tooltip("Link this to a GameTile asset")]
	public Claimable claimable;

	public bool Valid(TileTracker tracker, Vector3Int pos) {
		foreach (Claimable boardClaimable in tracker.GetTiles<Claimable>()) {
			if (!boardClaimable.claimed && boardClaimable.name.Equals(claimable.name)) return true;
		}
		CommandInput.Log($"No open {claimable.name} for {name} to claim.");
		return false;
	}

	void OnPlace() {
		claimable = gameTile.GetTracker().GetTiles<Claimable>()
			.Where(x => !x.claimed && x.name.Equals(claimable.name))
			.First();
		claimable.Claim(this.gameTile);
	}

	void OnRemove() {
		claimable.UnClaim(this.gameTile);
	}
}
