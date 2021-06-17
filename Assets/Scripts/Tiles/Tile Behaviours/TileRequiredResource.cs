using UnityEngine;
using System.Collections.Generic;

public class TileRequiredResource : TileBehaviour, ITileValidator {
	public List<ResourceAmount> resources;

	public bool Valid(TileTracker tracker, Vector3Int pos) {
		if (PlayerResources.Has(resources)) {
			return true;
		}

		CommandInput.Log($"Inadequate resources for tile {name}");
		CommandInput.Log(this);
		return false;
	}

	public void OnBuild() {
		PlayerResources.Remove(resources);
	}

	public override string ToString() {
		string ret = "Requires: ";
		foreach (var s in resources) {
			ret +="\n"+s;
		}
		return ret;
	}
}
