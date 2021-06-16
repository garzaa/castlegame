using UnityEngine;
using System.Collections;

public class TileTranspose : TileRedirection, ITileValidator {
	public Vector3Int fromRelative;
	public Vector3Int toRelative;

	void OnPlace() {
		StartCoroutine(AfterPlace());
	}

	IEnumerator AfterPlace() {
		yield return new WaitForEndOfFrame();
		TileTracker t = gameTile.GetTracker();
		Vector3Int source = gameTile.position + fromRelative;
		Vector3Int target = gameTile.position + toRelative;
		GameTile sourceTile = t.GetTileNoRedirect(source.x, source.y);
		GameTile targetTile = t.GetTileNoRedirect(target.x, target.y);
		gameTile.GetTracker().AddRedirect(new TileRedirect(sourceTile, targetTile));
	}

	override public bool Valid(TileTracker tracker, Vector3Int position) {
		bool valid = base.Valid(tracker, position);
		return valid && base.Valid(tracker, fromRelative);
	}

}
