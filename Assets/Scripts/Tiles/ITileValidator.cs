using UnityEngine;
using System.Collections.Generic;

public interface ITileValidator {
	bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message);
}
