using UnityEngine;
using System.Collections.Generic;

public class TileRequiredResource : TileBehaviour, ITileValidator {
	public List<ResourceAmount> resources;

	public bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message) {
		if (PlayerResources.Has(resources)) {
			return true;
		}
		string m = $"{this}";
		CommandInput.Log(m);
		message.Add(m);
		CommandInput.Log(this);
		return false;
	}

	public void OnBuild() {
		PlayerResources.Remove(resources);
	}

	public override string ToString() {
		string ret = "Requires";
		for (int i=0; i<resources.Count; i++) {
			ret +=" <color='#94fdff'>"+resources[i]+"</color>";
			if (resources.Count > 1 && i<resources.Count-1) ret += ", ";
		}
		ret += ".";
		return ret;
	}
}
