using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerResources : MonoBehaviour {
	Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
	static PlayerResources pr;

	#pragma warning disable 0649
	[SerializeField] List<ResourceAmount> startingResources;
	[SerializeField] GameObject resourceContainer;
	[SerializeField] GameObject resourceTemplate;
	#pragma warning restore 0649

	Dictionary<Resource, GameObject> resourceContainers = new Dictionary<Resource, GameObject>();

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
			pr.resourceContainers[r.resource].GetComponentInChildren<Text>().text = pr.resources[r.resource].ToString();
			if (log) CommandInput.Log($"{r.amount} {r.resource.name} removed");
		}
	}

	public static void Add(List<ResourceAmount> requirements, bool log=true) {
		foreach (ResourceAmount r in requirements) {
			if (pr.resources.ContainsKey(r.resource)) {
				pr.resources[r.resource] += r.amount;
				pr.resourceContainers[r.resource].GetComponentInChildren<Text>().text = pr.resources[r.resource].ToString();
			} else {
				pr.resources[r.resource] = r.amount;
				pr.AddResourceVisual(r);
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

	void AddResourceVisual(ResourceAmount resourceAmount) {
		GameObject g = Instantiate(resourceTemplate, resourceContainer.transform);
		g.SetActive(true);
		Image[] i = g.GetComponentsInChildren<Image>();
		i[i.Length-1].sprite = resourceAmount.resource.icon;
		i[i.Length-1].SetNativeSize();
		g.GetComponentInChildren<Text>().text = resourceAmount.amount.ToString();
		resourceContainers[resourceAmount.resource] = g;
	}
}
