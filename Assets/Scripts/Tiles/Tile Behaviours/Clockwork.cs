using UnityEngine;

public class Clockwork : TileBehaviour {
	public ClockworkTarget target;
	public ClockworkAction action;

	public void Tick() {
		if (!gameObject.activeSelf) return;
		action.Apply(target.GetTargets(gameTile.GetPosition(), gameTile.GetTracker()), gameTile);
	}

}
