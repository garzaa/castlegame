using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Tilemaps;

public class TileWarp : TileBehaviour, ITileHighlighter {
	public Vector3Int fromRelative;
	public Vector3Int toRelative;
	public TileWarpType warpType;
	
	Tile warpIcon;
	Vector3Int source;
	Vector3Int target;

	const string tileTemplatePath = "RuntimeLoaded/Tiles/";

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

	public TileHighlight GetHighlight() {
		List<Vector3Int> targets = new List<Vector3Int>();

		TileTracker tracker = gameTile.GetTracker();
		if (!tracker.BoardInBounds(source) || !tracker.BoardInBounds(target)) return null;

		// why does this need to happen HERE
		targets.Add(tracker.BoardToCell(source) + Vector3Int.left + Vector3Int.down);

		// lazy loading of highlight icon, only happens once
		if (warpIcon == null) {
			if (warpType.Equals(TileWarpType.REDIRECT)) {
				warpIcon = Resources.Load(tileTemplatePath+"WarpRedirectTile") as Tile;
			} else if (warpType.Equals(TileWarpType.COPY)) {
				warpIcon = Resources.Load(tileTemplatePath+"WarpCopyTile") as Tile;
			} else {
				warpIcon = Resources.Load(tileTemplatePath+"WarpReflectTile") as Tile;
			}
		}

		return new TileHighlight(warpIcon, targets);
	}
}
