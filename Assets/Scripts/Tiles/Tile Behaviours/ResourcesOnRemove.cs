using UnityEngine;
using System.Collections.Generic;

public class ResourcesOnRemove : TileBehaviour, IStat {
	[SerializeField] List<ResourceAmount> resources;

	void OnRemove() {
		PlayerResources.Add(resources);
	}

	public string Stat() {
		string s = "Yields ";
		foreach (ResourceAmount r in resources) {
			s += $"{r.amount} {r.resource.name} / ";
		}
		return s.TrimEnd('/', ' ');
	}
}
