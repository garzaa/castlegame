using UnityEngine;
using System.Collections.Generic;

public class Blueprints : CardSource {
	#pragma warning disable 0649
	[SerializeField] BlueprintCard cardTemplate;
	[SerializeField] List<GameTile> tiles;
	#pragma warning restore 0649

	TileTracker tileTracker;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
	}

	public List<GameTile> GetTiles() {
		return tiles;
	}

	override public List<CardBase> GetCards() {
		if (tileTracker == null) tileTracker = GameObject.FindObjectOfType<TileTracker>();
		List<CardBase> cards = new List<CardBase>();
		// for each blueprint
		// if it's got a blueprint unlock, and it's satisfied, then make a new card, initialize it, and add it to the hand
		foreach (GameTile tile in tiles) {
			BlueprintUnlock unlock = tile.GetComponent<BlueprintUnlock>();
			if (unlock && unlock.Unlocked(tileTracker)) {
				BlueprintCard card = Instantiate(cardTemplate);
				card.gameObject.SetActive(false);
				card.Initialize(tile);
				cards.Add(card);
			} else if (!unlock) {
				BlueprintCard card = Instantiate(cardTemplate);
				card.gameObject.SetActive(false);
				card.Initialize(tile);
				cards.Add(card);
			}
		}
		return cards;
	}
}
