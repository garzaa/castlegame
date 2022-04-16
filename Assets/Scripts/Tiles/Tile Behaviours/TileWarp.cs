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

	public TileHighlight GetHighlight(TileTracker tracker, Vector3Int boardPosition) {
		List<Vector3Int> sources = new List<Vector3Int>();

		Vector3Int statelessSource = boardPosition + fromRelative;
		Vector3Int statelessTarget = boardPosition + toRelative;

		if (!tracker.BoardInBounds(statelessSource) || !tracker.BoardInBounds(statelessTarget)) return null;

		// why does this need to happen HERE
		sources.Add(tracker.BoardToCell(statelessSource) + Vector3Int.left + Vector3Int.down);

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

		TileHighlight highlight = new TileHighlight(warpIcon, sources);

		// then add icons for the targets
		List<Vector3Int> targets = new List<Vector3Int>();
		targets.Add(tracker.BoardToCell(statelessTarget) + Vector3Int.left + Vector3Int.down);
		highlight.SetHighlight(Resources.Load(tileTemplatePath+"WarpTargetTile") as Tile, targets);

		return highlight;
	}
}
