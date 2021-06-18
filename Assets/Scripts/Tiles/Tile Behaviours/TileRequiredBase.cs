using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class TileRequiredBase : TileBehaviour, ITileValidator {
	public List<ScriptableTile> validBases;

	public bool Valid(TileTracker tracker, Vector3Int pos) {
		ScriptableTile currentBase = tracker.GetTile(pos, null).GetTile();
		if (!validBases.Contains(currentBase)) {
			List<string> validBases = this.validBases.Select(x => x.tileObject.name).ToList();
			CommandInput.Log("Invalid base for "+gameObject.name+". "+this);
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

	public override string ToString() {
		return "Build on: "+ PrettyList(validBases.Select(x => x.tileObject.name).ToList());
	}
}
