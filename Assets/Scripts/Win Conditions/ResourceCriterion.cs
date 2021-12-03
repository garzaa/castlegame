using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ResourceCriterion : GameStateRequirement {
	public List<ResourceAmount> resources;

	public override bool Satisfied(TileTracker tracker) {
		return PlayerResources.Has(resources);
	}
}
