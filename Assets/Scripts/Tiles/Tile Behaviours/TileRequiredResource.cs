using UnityEngine;
using System.Collections.Generic;

public class TileRequiredResource : TileCriterion {
	public List<ResourceAmount> resources;

	public override bool Valid(TileTracker tracker, Vector3Int pos) {
		if (PlayerResources.Has(resources)) {
			return true;
		}

		CommandInput.Log($"Inadequate resources for tile {name}");
		return false;
	}

	public void OnPlace() {
		PlayerResources.Remove(resources);
	}
}
