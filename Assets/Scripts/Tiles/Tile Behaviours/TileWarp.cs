using UnityEngine;
using System;
using System.Collections;

public class TileWarp : TileBehaviour {
	public Vector3Int fromRelative;
	public Vector3Int toRelative;
	public TileWarpType warpType;

	Vector3Int source;
	Vector3Int target;

	override public void OnPlace() {
		StartCoroutine(AfterPlace());
	}

	IEnumerator AfterPlace() {
		yield return new WaitForEndOfFrame();
		TileTracker t = gameTile.GetTracker();
		source = gameTile.position + fromRelative;
		target = gameTile.position + toRelative;
		gameTile.GetTracker().AddWarp(source, target, warpType);
	}

	override public void OnRemove() {
		gameTile.GetTracker().RemoveWarp(source, warpType);
	}

	public static string WarpToString(TileWarpType warpType) {
		if (warpType.Equals(TileWarpType.REDIRECT)) {
			return "Redirects";
		} else {
			return "Copies";
		}
	}

	void OnValidate() {
		if (warpType.Equals(TileWarpType.REFLECT) && toRelative!=Vector3Int.zero) {
			Debug.LogWarning("ToRelative on Reflection tile "+this.name+" will be ignored, as it's always the caller");
		}
	}
}
