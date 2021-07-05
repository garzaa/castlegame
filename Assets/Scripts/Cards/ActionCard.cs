using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;

public class ActionCard : CardBase {
	#pragma warning disable 0649
	#pragma warning restore 0649

	protected override void _TargetTile(Vector3 tileWorldPosition) {
		if (!boardTarget) {
			boardTarget = new GameObject();
			boardTarget.transform.parent = dragged.transform.parent;
		}
		float margin = 3f/(float)CameraZoom.GetZoomLevel();
		boardTarget.transform.position = Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.up * margin));
		lerp.target = boardTarget;
		targetingBoard = true;

		Vector3Int boardPosition = tileTracker.WorldToBoard(tileWorldPosition);

		// then hide the card for the tile preview
		animator.SetBool("PlacePreview", true);

		placementTest = ValidTarget(boardPosition);
		GameTile targetedTile = tileTracker.GetTileNoRedirect(boardPosition);
		tilemapVisuals.ShowTilePreview(GetPreviewTile(boardPosition, targetedTile), placementTest.Item1, tileWorldPosition);

		// show/hide invalid warning
		if (!placementTest.Item1) {
			base.ShowPlaceWarning(placementTest.Item2, tileWorldPosition, 0);
		} else {
			HidePlacementWarning();
		}
	}

	protected virtual Tuple<bool, string> ValidTarget(Vector3Int boardPosition) {
		throw new System.NotImplementedException();
	}

	protected virtual ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		throw new System.NotImplementedException();
	}
}
