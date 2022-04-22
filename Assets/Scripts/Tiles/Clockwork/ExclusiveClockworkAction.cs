using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class ExclusiveClockworkAction : ClockworkAction {
	public override void Apply(List<GameTile> tiles, GameTile from) {
		from.GetTracker().QueueExclusiveAction(this, new ClockworkApply(from, tiles, this));
	}

	virtual public Func<GameTile, float> GetPriorityComparator() {
		return tile => int.MaxValue;
	}

	public abstract List<GameTile> ExecuteApply(in List<GameTile> sortedTargets, in GameTile source);

}
