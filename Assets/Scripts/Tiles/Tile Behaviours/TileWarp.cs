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

	static readonly string tileTemplatePath = "RuntimeLoaded/Tiles/";

	override public void OnPlace() {
		StartCoroutine(AfterPlace());
	}

	public static Tile LoadWarpTile(string warpTypeTile) {
		return Resources.Load(tileTemplatePath+warpTypeTile) as Tile;
	}

	IEnumerator AfterPlace() {
		yield return new WaitForEndOfFrame();
		TileTracker t = gameTile.GetTracker();
		source = gameTile.boardPosition + fromRelative;
		target = gameTile.boardPosition + toRelative;
		gameTile.GetTracker().AddWarp(source, target, warpType);

		if (warpType.Equals(TileWarpType.REDIRECT)) {
			warpIcon = LoadWarpTile("WarpRedirectTile");
		} else if (warpType.Equals(TileWarpType.COPY)) {
			warpIcon = LoadWarpTile("WarpCopyTile");
		} else {
			warpIcon = LoadWarpTile("WarpReflectTile");
		}
	}

	override public void OnRemove(bool fromPlayer) {
		gameTile.GetTracker().RemoveWarp(source, target, warpType);
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

	public TileHighlight GetHighlight(TileTracker tracker = null) {
		List<Vector3Int> targets = new List<Vector3Int>();
		if (!tracker) tracker = gameTile.GetTracker();

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
