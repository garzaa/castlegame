using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public interface ITileHighlighter {
	TileHighlight GetHighlight(TileTracker tracker, Vector3Int boardPosition);
}
