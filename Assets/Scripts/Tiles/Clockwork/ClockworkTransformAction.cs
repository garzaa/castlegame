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

	public override List<GameTile> ExecuteApply(in List<GameTile> sortedTargets, in GameTile source) {
		List<GameTile> transformedTiles = new List<GameTile>();

		foreach (GameTile t in sortedTargets) {
			if (transformSet.ContainsKey(t.name)) {
				TileTracker tracker = t.GetTracker();
				GameTile g = tracker.ReplaceTile(t.boardPosition, transformSet[t.name]);
				transformedTiles.Add(g);
				if (transformedTiles.Count > limit && limit > 0) break;
			}
		}

		return transformedTiles;
	}


}

[System.Serializable]
public class TransformPair {
	public GameTile from;
	public ScriptableTile to;
}
