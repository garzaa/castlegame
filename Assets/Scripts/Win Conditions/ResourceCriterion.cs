using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ResourceCriterion : GameStateRequirement {
	public List<ResourceAmount> resources;

	public override bool Satisfied(TileTracker tracker) {
		return PlayerResources.Has(resources);
	}

	public override string ToString() {
		string s = "with: ";
		foreach (ResourceAmount r in resources) {
			s += r.amount + $" <color=\"#94fdff\">{r.resource}</color> "; 
		}
		return s;
	}
}
