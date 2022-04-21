using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

public abstract class ClockworkAction : ScriptableObject {
	public Tile icon;

	abstract public void Apply(List<GameTile> tiles, GameTile from);
}
