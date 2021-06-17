using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class WinConditionCriterion : MonoBehaviour {
	public abstract bool Satisfied(TileTracker tracker);
}
