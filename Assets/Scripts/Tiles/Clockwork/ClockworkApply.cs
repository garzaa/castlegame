using UnityEngine;
using System.Collections.Generic;

public class ClockworkApply {
	public GameTile sourceTile;
	public List<GameTile> targets = new List<GameTile>();
	public ExclusiveClockworkAction actionType;

	public ClockworkApply(GameTile f, List<GameTile> t, ExclusiveClockworkAction a) {
		this.sourceTile = f;
		this.targets = t;
		this.actionType = a;
	} 

	public override string ToString() {
		return $"{sourceTile.name} to {PrettyList(targets)}";
	}

	string PrettyList(List<GameTile> targets) {
		string s = "[ ";
		foreach (GameTile t in targets) {
			s += t.ToString() + " ";
		}
		return s + "]";
	}
}
