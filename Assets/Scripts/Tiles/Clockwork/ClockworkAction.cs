using UnityEngine;
using System.Collections.Generic;

public abstract class ClockworkAction : ScriptableObject {
	abstract public void Apply(List<GameTile> tiles, GameTile from);
}
