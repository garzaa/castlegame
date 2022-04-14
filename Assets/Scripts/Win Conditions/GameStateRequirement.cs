using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class GameStateRequirement : MonoBehaviour {
	public abstract bool Satisfied(TileTracker tracker);
	public override abstract string ToString();
}
