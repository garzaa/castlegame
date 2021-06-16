using UnityEngine;
using System.Collections;

public class TileTranspose : TileRedirection {
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
		Debug.Log($"Adding redirect from {sourceTile} to {targetTile}");
		gameTile.GetTracker().AddRedirect(new TileRedirect(sourceTile, targetTile));
	}

}
