using UnityEngine;
using System.Collections.Generic;

public class ResourcesOnRemove : TileBehaviour, IStat {
	#pragma warning disable 0649
	[SerializeField] List<ResourceAmount> resources;
	#pragma warning restore 0649

	override public void OnRemove(bool fromPlayer) {
		if (fromPlayer) PlayerResources.Add(resources);
	}

	public string Stat() {
		string s = "Yields ";
		foreach (ResourceAmount r in resources) {
			s += $"{r.amount} <color='#94fdff'>{r.resource.name}</color> / ";
		}
		return s.TrimEnd('/', ' ') + ".";
	}
}
