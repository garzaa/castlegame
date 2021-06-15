using UnityEngine;
using System.Collections.Generic;

public class ExclusiveClockworkAction : ClockworkAction {
	public override void Apply(List<GameTile> tiles, GameTile from) {
		from.GetTracker().QueueExclusiveAction(this, new ClockworkApply(from, tiles));
	}

	public virtual void ExecuteApply(ClockworkApply action) {
		throw new System.NotImplementedException();
	}
}
