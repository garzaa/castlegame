using UnityEngine;
using System.Collections.Generic;

public class TileRequiredResource : TileBehaviour, ITileValidator {
	public List<ResourceAmount> resources;

	public bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message) {
		if (PlayerResources.Has(resources)) {
			return true;
		}
		string m = $"Inadequate resources for tile {name}\n{this}";
		CommandInput.Log(m);
		message.Add(m);
		CommandInput.Log(this);
		return false;
	}

	public void OnBuild() {
		PlayerResources.Remove(resources);
	}

	public override string ToString() {
		string ret = "Requires:";
		foreach (var s in resources) {
			ret +=" "+s;
		}
		return ret;
	}
}
