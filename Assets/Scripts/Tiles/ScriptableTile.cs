using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Custom Tiles/Scriptable Tile")]
public class ScriptableTile : RuleTile {
	public GameTile backingScript;
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableTile))]
public class MyClassEditor : RuleTileEditor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
	}
}
#endif
