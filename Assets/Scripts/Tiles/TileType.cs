using UnityEngine;

[CreateAssetMenu(menuName ="Data Type/TileType")]
public class TileType : ScriptableObject {

	#pragma warning disable 0649
	[SerializeField] TileType ancestor;
	#pragma warning restore 0649

	public bool IsType(TileType tileType) {
		if (name.Equals(tileType.name)) return true;
		else if (ancestor) return ancestor.IsType(tileType);
		else return false;
	}

	void OnValidate() {
		if (!ancestor) return;
		if (this.name == ancestor.name) {
			Debug.LogWarning("Enjoy your infinite loops idiot");
		}
	}
}
