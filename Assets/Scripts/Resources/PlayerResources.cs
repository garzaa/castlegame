using UnityEngine;
using System.Collections.Generic;

public class PlayerResources : MonoBehaviour {
	Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
	static PlayerResources pr;

	[SerializeField] List<ResourceAmount> startingResources;

	void Start() {
		pr = this;
		Add(startingResources, log:false);
	}

	public static bool Has(List<ResourceAmount> requirements) {
		foreach (ResourceAmount r in requirements) {
			if (!pr.resources.ContainsKey(r.resource)) {
				return false;
			}
			if (pr.resources[r.resource] < r.amount) {
				return false;
			}
		}
		return true;
	}

	public static void Remove(List<ResourceAmount> requirements, bool log=true) {
		foreach (ResourceAmount r in requirements) {
			pr.resources[r.resource] -= r.amount;
			pr.resources[r.resource] = Mathf.Max(pr.resources[r.resource], 0);
			if (log) CommandInput.Log($"{r.amount} {r.resource.name} removed");
		}
	}

	public static void Add(List<ResourceAmount> requirements, bool log=true) {
		foreach (ResourceAmount r in requirements) {
			if (pr.resources.ContainsKey(r.resource)) {
				pr.resources[r.resource] += r.amount;
			} else {
				pr.resources[r.resource] = r.amount;
			}
			if (log) CommandInput.Log($"{r.amount} {r.resource.name} added");
		}
	}

	public static string Stat() {
		string s = "";
		foreach (Resource r in pr.resources.Keys) {
			s += r.name + " : " + pr.resources[r] + "\n";
		}
		return s.TrimEnd('\r', '\n');
	}
}
