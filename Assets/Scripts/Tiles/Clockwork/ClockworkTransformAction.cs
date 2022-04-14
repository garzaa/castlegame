using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Clockwork/Transform Action")]
public class ClockworkTransformAction : ExclusiveClockworkAction {
	public int limit = -1;
	public TransformPair[] transforms;

	// faster structure that can't be serialized
	Dictionary<string, ScriptableTile> transformSet= new Dictionary<string, ScriptableTile>();

	void OnValidate() {
		foreach (TransformPair p in transforms) {
			transformSet[p.from.name] = p.to;
		}
	}

	public override void ExecuteApply(ClockworkApply action) {
		int amountTransformed = 0;

		foreach (GameTile t in action.targets) {
			if (transformSet.ContainsKey(t.name)) {
				TileTracker tracker = t.GetTracker();
				tracker.ReplaceTile(t.boardPosition, transformSet[t.name]);
				if (++amountTransformed > limit && limit > 0) break;
			}
		}
	}


}

[System.Serializable]
public class TransformPair {
	public GameTile from;
	public ScriptableTile to;
}
