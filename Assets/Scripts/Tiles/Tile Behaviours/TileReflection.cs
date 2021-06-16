using UnityEngine;

public class TileReflection : TileRedirection {
	void OnPlace() {
		gameTile.GetTracker().AddRedirect(new TileRedirect(this.gameTile, null));
	}
}
