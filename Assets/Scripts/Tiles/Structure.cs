using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Tile Behaviors/Structure")]
public class Structure : GameTile {
	int age;

	virtual public void Decay() {
		age++;
	}

	public override void Initialize(Tilemap tilemap, Vector3Int position) {
		base.Initialize(tilemap, position);
		this.age = 0;
	}
}
