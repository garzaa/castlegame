using UnityEngine;

public class Clockwork : TileBehaviour {
	// has action and target fields - repair, age, what else...
	// prerequisites? like the house needs a garden next to it

	// actions and targets need to be scriptable objects that take in a position and act on them
	// target returns a list of GameTiles given an input position and a tile tracker
	// actions, given a list of GameTiles, does something to them

	// TODO: the extra house isn't repairing the garden when it should be
	// also todo: the house needs a garden to do the repairs after it's up

	public ClockworkTarget target;
	public ClockworkAction action;

	public void Tick() {
		if (!gameObject.activeSelf) return;
		action.Apply(target.GetTargets(gameTile.GetPosition(), gameTile.GetTracker()), gameTile);
	}

}
