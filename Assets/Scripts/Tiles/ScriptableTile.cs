using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ScriptableTile : RuleTile {
	public GameObject tileObject;
}

#if UNITY_EDITOR
[CustomEditor(typeof(ScriptableTile))]
public class MyClassEditor : RuleTileEditor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
	}
}
#endif
