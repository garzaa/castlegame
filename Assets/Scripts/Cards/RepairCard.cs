using UnityEngine;
using System;

public class RepairCard : ActionCard {
	public TileType repairableType;

	protected override Tuple<bool, string> ValidTarget(Vector3Int boardPosition) {
		string message = "";
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		if (tile == null) {
			return new Tuple<bool, string>(false, message);
		}
		TileAge age = tile.GetComponent<TileAge>();
		bool repairable = age && tile.IsTileType(repairableType);
		if (!repairable) message = $"{tile.name} can't be repaired.";
		if (age && age.GetAge() <= 0) {
			repairable = false;
			message = $"{tile.name} is too new to be repaired.";
		}
		return new Tuple<bool, string>(repairable, message);
	}

	protected override ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		if (!targetedTile) return null;
		return targetedTile.GetDefaultTile();
	}

	protected override void OnDrop(Vector3Int boardPosition) {
		TileAge age = tileTracker.GetTileNoRedirect(boardPosition).GetComponent<TileAge>();
		age.Repair();
		ReturnToHand();
	}
}
