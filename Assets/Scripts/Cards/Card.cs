using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class Card : MonoBehaviour {

	#pragma warning disable 0649
	[SerializeField] Text tileName;
	[SerializeField] Transform tileInfoContainer;

	[Header("Templates")]
	[SerializeField] GameObject infoLine;
	#pragma warning restore 0649


	void Initialize(GameTile tile) {
		foreach (Transform child in tileInfoContainer.transform) {
			GameObject.Destroy(child.gameObject);
		}

		tileName.text = tile.name;

		foreach (ICardStat i in tile.GetComponents<ICardStat>()) {
			string s = i.Stat();
			if (string.IsNullOrEmpty(s)) continue;
			GameObject g = Instantiate(infoLine, tileInfoContainer);
			g.GetComponent<Text>().text = i.Stat();
		}

		foreach (LayoutGroup g in GetComponentsInChildren<LayoutGroup>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(g.GetComponent<RectTransform>());
		}
	}
}
