using UnityEngine;
using System.Collections.Generic;

public class ResourcesOnRemove : TileBehaviour, IStat {
	#pragma warning disable 0649
	[SerializeField] List<ResourceAmount> resources;
	#pragma warning restore 0649

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
