using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName="Clockwork/Target DecayTo")]
public class ClockworkTargetDecayTo : ClockworkTargetNetworked {
	#pragma warning disable 0649
	[SerializeField]
	List<GameTile> decayToTiles;
	#pragma warning restore 0649

	override protected bool IsTargetable(GameTile tile) {
		if (base.IsTargetable(tile)) {
			TileDecay[] decays = tile.GetComponents<TileDecay>();
			foreach (TileDecay d in decays) {
				if (decayToTiles.Contains(d.GetDecayTo().tileObject.GetComponent<GameTile>())) {
					return true;
				}
			}
		}

		return false;
	}
}
