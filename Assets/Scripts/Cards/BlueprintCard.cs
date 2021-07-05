using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(TargetLerp))]
public class BlueprintCard : CardBase {

	#pragma warning disable 0649
	[Header("Linked Data")]
	[SerializeField] GameTile gameTile;
	[SerializeField] Text tileName;
	[SerializeField] Image tileIcon;
	[SerializeField] Transform tileInfoContainer;
	[SerializeField] Transform resourceContainer;

	[Header("Templates")]
	[SerializeField] GameObject infoLine;
	[SerializeField] GameObject resourceRequirement;
	#pragma warning restore 0649

	override protected void Start() {
		base.Start();
		if (gameTile) Initialize(gameTile);
	}

	override protected void _TargetTile(Vector3 tileWorldPosition) {
		if (!boardTarget) {
			boardTarget = new GameObject();
			boardTarget.transform.parent = dragged.transform.parent;
		}
		float margin = 3f/(float)CameraZoom.GetZoomLevel();
		boardTarget.transform.position = Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.up * margin));
		lerp.SetTarget(boardTarget);
		targetingBoard = true;

		animator.SetBool("PlacePreview", true);
		placementTest = tileTracker.ValidPlacement(gameTile, tileTracker.WorldToBoard(tileWorldPosition));

		tilemapVisuals.ShowTilePreview(this.gameTile.GetDefaultTile(), placementTest.Item1, tileWorldPosition);

		// show/hide invalid warning
		if (!placementTest.Item1) {
			base.ShowPlaceWarning(placementTest.Item2, tileWorldPosition, 0);
		} else {
			HidePlacementWarning();
		}
	}

	protected override void OnDrop(Vector3Int boardPosition) {
		tileTracker.ReplaceTile(boardPosition, this.gameTile.GetDefaultTile());
		Destroy(this.gameObject);
	}

	public void Initialize(GameTile tile) {
		ClearChildren(tileInfoContainer.transform);
		ClearChildren(resourceContainer.transform);

		tileIcon.sprite = tile.GetDefaultTile().m_DefaultSprite;
		tileIcon.SetNativeSize();
		gameTile = tile;

		tileName.text = tile.name;

		foreach (TileRequiredResource resourceList in tile.GetComponents<TileRequiredResource>()) {
			foreach (ResourceAmount resource in resourceList.resources) {
				GameObject g = Instantiate(resourceRequirement, resourceContainer);
				g.GetComponentInChildren<Text>().text = resource.amount.ToString();
				g.GetComponentInChildren<Image>().sprite = resource.resource.icon;
			}
		}

		foreach (ICardStat i in tile.GetComponents<ICardStat>()) {
			string s = i.Stat();
			if (string.IsNullOrEmpty(s)) continue;
			GameObject g = Instantiate(infoLine, tileInfoContainer);
			g.GetComponent<Text>().text = i.Stat();
		}

		RebuildUI();
	}
}
