using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class WinConditionCriterion : MonoBehaviour {
	public abstract bool Satisfied(Tilemap tilemap);
}
