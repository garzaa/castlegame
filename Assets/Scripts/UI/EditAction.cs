using UnityEngine;

public class EditAction : ActionButton {
	protected override void TargetTile(Vector3 tileWorldPosition) {
		Vector3Int boardPosition = tileTracker.WorldToBoard(tileWorldPosition);
		PlacementTestResult r = TestPlacement(boardPosition);
		GameTile targetedTile = tileTracker.GetTileNoRedirect(boardPosition);
		tilemapVisuals.ShowTilePreview(GetPreviewTile(boardPosition, targetedTile), r.valid, tileWorldPosition);

		if (!r.valid) {
			ShowActionWarning(r.message, tileWorldPosition, 0);
		} else {
			HideActionWarning();
		}
	}

	protected virtual ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		throw new System.NotImplementedException();
	}
}
