using UnityEngine;
using System.Collections.Generic;

public class Blueprints : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] List<GameTile> tiles;
	#pragma warning restore 0649

	public List<GameTile> GetTiles() {
		return tiles;
	}
}
