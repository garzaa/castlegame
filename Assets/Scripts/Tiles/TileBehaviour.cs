using UnityEngine;

[RequireComponent(typeof(GameTile))]
public class TileBehaviour : MonoBehaviour {
	protected GameTile gameTile;

	virtual protected void Start() {
		gameTile = GetComponent<GameTile>();
	}
}
