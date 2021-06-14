using UnityEngine;

[RequireComponent(typeof(GameTile))]
public class TileBehaviour : MonoBehaviour {
	public GameTile gameTile { get; private set; }

	virtual protected void Awake() {
		gameTile = GetComponent<GameTile>();
	}

	virtual protected void Start() {
		
	}
}
