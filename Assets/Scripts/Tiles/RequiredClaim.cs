using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class RequiredClaim : TileBehaviour, ITicker, ITileValidator, ITileHighlighter, IStat, ICardStat {
	[Tooltip("Link this to a GameTile asset")]
	public Claimable claimable;
	public Tile claimedTileIcon;
	public bool requiredToPlace = true;
	Claimable claimed;

	override protected void Start() {
		base.Start();
	}

	public void Tick() {
		if (!claimed) {
			Claim();
		}
	}

	public bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message) {
		if (!requiredToPlace) return true;
		foreach (Claimable boardClaimable in tracker.GetTiles<Claimable>()) {
			if (!boardClaimable.claimed && boardClaimable.name.Equals(claimable.name)) return true;
		}
		string m = $"No open {claimable.name} for {name} to claim.";
		message.Add(m);
		CommandInput.Log(m);
		return false;
	}

	override public void OnPlace() {
		Claim();
	}

	override public void OnRemove(bool fromPlayer) {
		claimable.UnClaim(this.gameTile);
	}

	public TileHighlight GetHighlight(TileTracker tracker, Vector3Int boardPosition) {
		if (!claimed) return null;
		return new TileHighlight (
			claimedTileIcon,
			new List<Vector3Int>(new Vector3Int[] {claimed.gameTile.gridPosition})
		);
	}

	public string Stat() {
		if (requiredToPlace) return $"Claims 1 <color='#94fdff'>{claimable.gameObject.name}</color>.";
		else return $"Claims first available <color='#94fdff'>{claimable.gameObject.name}</color>.";
	}

	void Claim() {
		claimed = gameTile.GetTracker().GetTiles<Claimable>()
			.FirstOrDefault(x => !x.claimed && x.name.Equals(claimable.name));

		if (claimed == null && !requiredToPlace) {
			// if not required to place (like a garden) it will just claim whenever something is immediately available on a tick
			return;
		}

		// this might not have been initialized...what to do
		// second pass of initializations
		claimed.Claim(this.gameTile);
		gameTile.GetTracker().SendBoardChanged();
	}
}
