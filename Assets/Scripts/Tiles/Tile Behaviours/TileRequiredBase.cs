using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class TileRequiredBase : TileBehaviour, ITileValidator, ICardStat {
	public List<ScriptableTile> validBases;

	public bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message) {
		ScriptableTile currentBase = tracker.GetTileNoRedirect(pos).GetDefaultTile();
		if (!validBases.Contains(currentBase)) {
			List<string> validBases = this.validBases.Select(x => x.tileObject.name).ToList();
			string m = "Invalid base for "+gameObject.name+". "+this;
			message.Add(m);
			CommandInput.Log(m);
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

	public string Stat() {
		if (validBases.Count > 1) {
			return this.ToString();
		} else {
			return $"Build on <color='#94fdff'>{validBases[0].tileObject.name}</color>.";
		}
	}
}
