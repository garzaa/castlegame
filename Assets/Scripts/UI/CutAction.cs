using UnityEngine;

public class CutAction : EditAction {
	#pragma warning disable 0649
	[SerializeField] TileType razeType;
	[SerializeField] ScriptableTile defaultRazeTo;
	#pragma warning restore 0649

	protected override void ApplyAction(Vector3Int boardPosition) {
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		ScriptableTile replacementTile;
		if (tile.IsTileType(razeType)) {
			replacementTile = GetRazeTile(tile);
		} else {
			replacementTile = tile.GetComponent<TileCuttable>().cutTo;
		}
		tileTracker.ReplaceTile(boardPosition, replacementTile);
	}

	protected override PlacementTestResult TestPlacement(Vector3Int boardPosition) {
		string message = "";
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		if (tile == null) {
			return new PlacementTestResult(false, message);
		}

		if (tile.IsTileType(razeType)) {
			return new PlacementTestResult(true, message);
		}

		bool cuttable = tile.GetComponent<TileCuttable>() != null;
		if (!cuttable) message = $"{tile.name} can't be cut";
		return new PlacementTestResult(cuttable, message);
	}

	protected override ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		if (!targetedTile) return null;
		if (targetedTile.IsTileType(razeType)) return GetRazeTile(targetedTile);
		TileCuttable cut = targetedTile.GetComponent<TileCuttable>();
		if (!cut) return null;
		else return cut.cutTo;
	}

	ScriptableTile GetRazeTile(GameTile tile) {
		TileRequiredBase tileBase = tile.GetComponent<TileRequiredBase>();
		if (tileBase) return tileBase.validBases[0];
		else return defaultRazeTo;
	}
}
