using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TileRequiredBase : TileBehaviour, ITileValidator {
	public List<ScriptableTile> validBases;

	public bool Valid(TileTracker tracker, Vector3Int pos) {
		ScriptableTile currentBase = tracker.GetTile(pos, null).GetTile();
		if (!validBases.Contains(currentBase)) {
			List<string> validBases = this.validBases.Select(x => x.tileObject.name).ToList();
			CommandInput.Log("Invalid base for "+gameObject.name+". Build on: "+ PrettyList(validBases));
			return false;
		}
		return true;
	}

	public string PrettyList(List<string> l) {
		string s = "[ ";
		foreach (string x in l) {
			s += x + " ";
		}
		s += "]";
		return s;
	}
}
