using UnityEngine;
using System.Collections.Generic;

public abstract class ExclusiveClockworkAction : ClockworkAction {
	public override void Apply(List<GameTile> tiles, GameTile from) {
		from.GetTracker().QueueExclusiveAction(this, new ClockworkApply(from, tiles, this));
	}

	public abstract void ExecuteApply(ClockworkApply action);

}
