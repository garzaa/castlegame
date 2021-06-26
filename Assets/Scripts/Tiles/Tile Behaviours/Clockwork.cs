using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class Clockwork : TileBehaviour, ITileHighlighter {
	public ClockworkTarget target;
	public ClockworkAction action;

	public void Tick() {
		if (!gameObject.activeSelf) return;
		action.Apply(target.GetTargets(gameTile.position, gameTile.GetTracker()), gameTile);
	}

	public string Stat() {
		return target.GetTargetInfo(gameTile.position, gameTile.GetTracker());
	}

	public TileHighlight GetHighlight() {
		return new TileHighlight(
			action.icon,
			target.GetTargets(gameTile.position, gameTile.GetTracker()).Select(x => x.gridPosition).ToList()
		);
	}
}
