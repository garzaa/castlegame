using UnityEngine;

public abstract class TileCriterion : MonoBehaviour {
	abstract public bool Valid(TileTracker tracker, Vector3Int pos);
}
