using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ScriptableTile : RuleTile {
	public GameObject tileObject;
	public TileType[] tileWith;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		base.GetTileData(position, tilemap, ref tileData);
	}

	public override bool RuleMatch(int neighbor, TileBase other) {
		if (tileWith != null && tileWith.Length>0 && other is ScriptableTile) {
			ScriptableTile otherTile = other as ScriptableTile;
			GameTile otherGameTile = otherTile.tileObject.GetComponent<GameTile>();
			bool canTile = other == this;
			foreach (TileType tileType in tileWith) {
				canTile = canTile || otherGameTile.IsTileType(tileType);
			}

			switch (neighbor) {
				case TilingRuleOutput.Neighbor.This: return canTile;
				case TilingRuleOutput.Neighbor.NotThis: return !canTile;
			}
		}

		return base.RuleMatch(neighbor, other);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableTile))]
public class MyClassEditor : RuleTileEditor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
	}
}
#endif
