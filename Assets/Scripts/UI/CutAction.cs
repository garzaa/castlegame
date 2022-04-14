using UnityEngine;

public class CutAction : EditAction {
	#pragma warning disable 0649
	[SerializeField] TileType razeType;
	[SerializeField] ScriptableTile defaultRazeTo;
	#pragma warning restore 0649

	protected override void ApplyAction(Vector3Int boardPosition) {
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		tileTracker.ReplaceTile(boardPosition, GetCutTo(tile), fromPlayer: true);
	}

	protected override PlacementTestResult TestPlacement(Vector3Int boardPosition) {
		string message = "";
		GameTile tile = tileTracker.GetTileNoRedirect(boardPosition);
		if (tile == null) {
			return new PlacementTestResult(false, message);
		}

		bool cuttable = tile.GetComponent<TileCuttable>() != null && tile.GetComponent<TileCuttable>().Cuttable();

		if (tile.IsTileType(razeType) && cuttable) {
			return new PlacementTestResult(true, message);
		}

		if (!cuttable) message = $"{tile.name} can't be cut";
		return new PlacementTestResult(cuttable, message);
	}

	ScriptableTile GetCutTo(GameTile currentTile) {
		TileCuttable cuttable = currentTile.GetComponent<TileCuttable>();
		if (currentTile.IsTileType(razeType) && cuttable==null && cuttable.Cuttable()) {
			return GetRazeTile(currentTile);
		} else if (cuttable != null) {
			return cuttable.cutTo;
		} else {
			return null;
		}
	}

	protected override ScriptableTile GetPreviewTile(Vector3Int boardPosition, GameTile targetedTile) {
		if (!targetedTile) return null;
		else return GetCutTo(targetedTile);
	}

	ScriptableTile GetRazeTile(GameTile tile) {
		TileRequiredBase tileBase = tile.GetComponent<TileRequiredBase>();
		if (tileBase) return tileBase.validBases[0];
		else return defaultRazeTo;
	}
}
