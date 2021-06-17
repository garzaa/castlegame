using UnityEngine;
using UnityEngine.Tilemaps;

public class WinCondition : MonoBehaviour {
	public bool Check(Tilemap tilemap) {
		foreach (WinConditionCriterion c in GetComponents<WinConditionCriterion>()) {
			if (!c.Satisfied(tilemap)) {
				return false;
			}
		}

		return true;
	}
}
