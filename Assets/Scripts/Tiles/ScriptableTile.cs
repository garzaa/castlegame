using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ScriptableTile : RuleTile {
	public GameObject tileObject;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		base.GetTileData(position, tilemap, ref tileData);
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
