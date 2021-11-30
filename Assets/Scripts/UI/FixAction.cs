using UnityEngine;

public class FixAction : EditAction {
	#pragma warning disable 0649
	[SerializeField] TileType repairableType;
	#pragma warning restore 0649

	protected override void ApplyAction(Vector3Int boardPosition) {
		TileAge age = tileTracker.GetTileNoRedirect(boardPosition).GetComponent<TileAge>();
		age.Repair();
	}

	protected override PlacementTestResult TestPlacement(Vector3Int boardPosition) {
		string message = "";
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		if (tile == null) {
			return new PlacementTestResult(false, message);
		}
		TileAge age = tile.GetComponent<TileAge>();
		bool repairable = age && tile.IsTileType(repairableType);
		if (!repairable) message = $"{tile.name} can't be repaired.";
		if (age && age.GetAge() <= 0) {
			repairable = false;
			message = $"{tile.name} is too new to be repaired.";
		}
		return new PlacementTestResult(repairable, message);
	}

	protected override ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		if (!targetedTile) return null;
		return targetedTile.GetDefaultTile();
	}
}
