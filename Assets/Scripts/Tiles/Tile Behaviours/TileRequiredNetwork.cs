using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileRequiredNetwork : TileBehaviour, ITileValidator, ICardStat {

	#pragma warning disable 0649
	public ClockworkTargetNetworked filter;
	#pragma warning restore 0649
	
	public bool Valid(TileTracker tracker, Vector3Int pos, ref List<string> message) {
		List<GameTile> targets = new List<GameTile>();

		// this is kinda slow but whatever
		GameTile target = filter.GetTargets(pos, tracker).FirstOrDefault();

		if (target == null) {
			message.Add("Must be networked with a <color='#94fdff'>"+filter.GetTargetType()+"</color> tile.");
			return false;
		}

		return true;
	}

	public string Stat() {
		return "Must be networked with a "+filter.GetTargetType()+" tile.";
	}
}
