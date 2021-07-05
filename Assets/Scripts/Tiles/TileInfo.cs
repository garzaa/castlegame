using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TileInfo : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] Text tileName;
	[SerializeField] Text tileLocation;
	[SerializeField] GameObject tileAgeContainer;
	[SerializeField] Text tileAge;
	[SerializeField] Transform tileInfoContainer;

	[Header("Templates")]
	[SerializeField] GameObject infoLine;
	#pragma warning restore 0649

	TilemapVisuals tilemapVisuals;

	void Start() {
		tilemapVisuals = GetComponentInParent<TilemapVisuals>();
	}

	public void Initialize(GameTile tile) {
		foreach (Transform child in tileInfoContainer.transform) {
			GameObject.Destroy(child.gameObject);
		}

		GetComponent<WorldPointCanvas>().position = tile.worldPosition;
		
		tileName.text = tile.name;
		if (tile.GetComponent<TileAge>()) {
			tileAge.text = tile.GetComponent<TileAge>().GetAge().ToString();
		} else {
			tileAgeContainer.SetActive(false);
		}

		tileLocation.text = tile.GetTracker().PosToStr(tile.position);

		foreach (IStat i in tile.GetComponents<IStat>()) {
			string s = i.Stat();
			if (string.IsNullOrEmpty(s)) continue;
			GameObject g = Instantiate(infoLine, tileInfoContainer);
			g.GetComponent<Text>().text = i.Stat();
		}

		foreach (LayoutGroup g in GetComponentsInChildren<LayoutGroup>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(g.GetComponent<RectTransform>());
		}
	}

	// called from button
	public void Close() {
		tilemapVisuals.HideInfoBubble();
	}
}
