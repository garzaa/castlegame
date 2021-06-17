using UnityEngine;

public class GameEventOnRemove : TileBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameEvent target;

	void OnRemove() {
		target.Raise();
	}
}
