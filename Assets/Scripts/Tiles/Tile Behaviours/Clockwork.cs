using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class Clockwork : TileBehaviour, ITicker, ITileHighlighter, IConsoleStat, IStat {
	public string shortName;
	public ClockworkTarget target;
	public ClockworkAction action;

	public void Tick() {
		if (!gameObject.activeSelf) return;
		// add the copies here??
		// foreach target, if tile tracker has a copy, then add the copy as well
		action.Apply(target.GetTargets(gameTile.boardPosition, gameTile.GetTracker()), gameTile);
	}

	public string Stat() {
		return "<color='#94fdff'>"+shortName + "</color> " + target.GetTargetInfo(gameTile.boardPosition, gameTile.GetTracker());
	}

	public TileHighlight GetHighlight(TileTracker tracker, Vector3Int boardPosition) {
		return new TileHighlight(
			action.icon,
			target.GetTargets(boardPosition, tracker).Select(x => x.gridPosition).ToList()
		);
	}

	// this can also be called from a preview tile, so have optional args for things already in place
	public List<GameTile> GetPossibleTargets(Vector3Int boardPosition, TileTracker tracker) {
		return target.GetTargets(boardPosition, tracker);
	}

	public List<GameTile> GetSearchArea(Vector3Int boardPosition, TileTracker tracker) {
		return target.GetTargetsWithVisited(boardPosition, tracker);
	}
}
