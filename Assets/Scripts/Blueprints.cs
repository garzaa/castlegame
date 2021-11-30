using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Blueprints : CardSource {
	#pragma warning disable 0649
	[SerializeField] BlueprintCard cardTemplate;
	[SerializeField] List<GameTile> tiles;
	// TODO: add a buttonTemplate thing
	#pragma warning restore 0649

	HashSet<GameTile> lastSourced = new HashSet<GameTile>();

	TileTracker tileTracker;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
	}

	public List<GameTile> GetTiles() {
		return tiles;
	}

	override public List<CardBase> GetCards() {
		lastSourced.Clear();
		if (tileTracker == null) tileTracker = GameObject.FindObjectOfType<TileTracker>();
		List<CardBase> cards = new List<CardBase>();
		// for each blueprint
		// if it's got a blueprint unlock, and it's satisfied, then make a new card, initialize it, and add it to the hand
		foreach (GameTile tile in tiles) {
			BlueprintUnlock unlock = tile.GetComponent<BlueprintUnlock>();
			if (unlock && unlock.Unlocked(tileTracker)) {
				cards.Add(SpawnCard(tile));
				lastSourced.Add(tile);
			} else if (!unlock) {
				cards.Add(SpawnCard(tile));
				lastSourced.Add(tile);
			}
		}
		return cards;
	}

	override public List<CardBase> GetMidRoundCards() {
		List<CardBase> newCards = new List<CardBase>();
		foreach (GameTile tile in tiles) {
			BlueprintUnlock unlock = tile.GetComponent<BlueprintUnlock>();
			if (unlock && !lastSourced.Contains(tile) && unlock.Unlocked(tileTracker)) {
				newCards.Add(SpawnCard(tile));
				lastSourced.Add(tile);
			}
		}
		return newCards;
	}

	BlueprintCard SpawnCard(GameTile tile) {
		BlueprintCard card = Instantiate(cardTemplate);
		card.gameObject.SetActive(false);
		card.Initialize(tile);
		return card;
	}
}
