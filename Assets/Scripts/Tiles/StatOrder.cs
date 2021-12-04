using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

public class StatOrder {

	static Type[] statTypes = {
		typeof(GameTile),
		typeof(TileRequiredBase),
		typeof(Claimable),
		typeof(NeighborCriterion),
		typeof(TileCuttable),
		typeof(ResourcesOnRemove),
		typeof(RequiredClaim),
		typeof(TileDecay),
		typeof(NeighborDecay),
		typeof(BlueprintUnlock),
	};

	public static IStat[] OrderStats(IStat[] stats) {
		return stats.OrderBy(x => {
			int i = Array.IndexOf(statTypes, x.GetType());
			if (i == -1) return int.MaxValue;
			return i;
		}).ToArray();
	}

	public static ICardStat[] OrderCardStats(ICardStat[] stats) {
		return stats.OrderBy(x => {
			int i = Array.IndexOf(statTypes, x.GetType());
			Debug.Log(i);
			if (i == -1) return int.MaxValue;
			return i;
		}).ToArray();
	}
}
