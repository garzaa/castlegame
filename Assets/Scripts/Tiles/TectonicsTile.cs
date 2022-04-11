using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class TectonicsTile : RuleTile {
	public GameObject tileObject;

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		base.GetTileData(position, tilemap, ref tileData);
		// don't lock transform
		tileData.flags = TileFlags.None;
	}
}
