using UnityEngine;
using System;

public class CutCard : ActionCard {
	protected override Tuple<bool, string> ValidTarget(Vector3Int boardPosition) {
		string message = "";
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		if (tile == null) {
			return new Tuple<bool, string>(false, message);
		}
		bool cuttable = tile.GetComponent<TileCuttable>() != null;
		if (!cuttable) message = $"{tile.name} can't be cut";
		return new Tuple<bool, string>(cuttable, message);
	}

	protected override ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		if (!targetedTile) return null;
		TileCuttable cut = targetedTile.GetComponent<TileCuttable>();
		if (!cut) return targetedTile.GetDefaultTile();
		else return cut.cutTo;
	}

	protected override void OnDrop(Vector3Int boardPosition) {
		TileCuttable cut = tileTracker.GetTileNoRedirect(boardPosition).GetComponent<TileCuttable>();
		tileTracker.ReplaceTile(boardPosition, cut.cutTo);
		Destroy(this);
	}
}
