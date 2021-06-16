using UnityEngine;

public interface ITileValidator {
	bool Valid(TileTracker tracker, Vector3Int pos);
}
