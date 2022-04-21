using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/*
	If there's a structure that can apply an exclusive action to two tiles
	with the same action's registered priority, pick tiles that only THIS
	exclusive action can target in the case of a tiebreaker
	This structure exists to make that happen
*/

public class ExclusiveActionMatrix : MonoBehaviour {
	List<ClockworkApply> applications;
	Dictionary<GameTile, int> targeterAmounts;

	public ExclusiveActionMatrix() {
		applications = new List<ClockworkApply>();
		targeterAmounts = new Dictionary<GameTile, int>();
	}

	public void Add(ClockworkApply spec) {
		applications.Add(spec);
		foreach (GameTile target in spec.targets) {
			if (targeterAmounts.ContainsKey(target)) {
				targeterAmounts[target]++;
			} else {
				targeterAmounts[target] = 1;
			}
		}
	}

	public void Resolve() {
		SortApplicationActions();

		List<ClockworkApply> singularApplications;

		// do this instead of iterating since we'll be removing things each loop
		while ((singularApplications = GetSingularApplications()).Count > 0) {
			ClockworkApply application = singularApplications[0];
			GameTile sourceTile = application.sourceTile;
			
			// apply the action to that tile
			List<GameTile> affectedTiles = application.actionType.ExecuteApply(application.targets, application.sourceTile);

			// then remove all references to those tiles from the list of actions
			PruneTargetsFromActions(affectedTiles);
		}

		// final pass for removal of singular actions
		applications.RemoveAll(x => x.targets.Count == 0);

		// then do the same thing for the rest of the actions with multiple targets
		while (applications.Count > 0) {
			// always pick the one with the least amount of targets
			applications = applications.OrderBy(a => a.targets.Count).ToList();
			List<GameTile> affectedTiles = applications[0].actionType.ExecuteApply(applications[0].targets, applications[0].sourceTile);
			PruneTargetsFromActions(affectedTiles);
			applications.RemoveAt(0);
		}
	}

	void SortApplicationActions() {
		// sort by action priority and then by targeter numbers for each tile
		foreach (ClockworkApply apply in applications) {
			List<GameTile> sortedTargets = apply.targets
				.OrderBy(apply.actionType.GetOrderingKey())
				.ThenBy(tile => targeterAmounts[tile])
				.ToList();
		}
	}

	void PruneTargetsFromActions(List<GameTile> affectedTiles) {
		foreach (ClockworkApply action in applications) {
			action.targets.RemoveAll(x => affectedTiles.Contains(x));
		}
		applications.RemoveAll(x => x.targets.Count == 0);
	}

	List<ClockworkApply> GetSingularApplications() {
		return applications.Where(x => x.targets.Count == 1).ToList();
	}
}
