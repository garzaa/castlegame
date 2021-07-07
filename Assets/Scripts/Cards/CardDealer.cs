using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CardDealer : MonoBehaviour {
	GameTile keep;
	GameObject keepDiscardPile;
	TileTracker tileTracker;
	const float cardInterval = 0.1f;

	bool firstDay = true;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
	}

	public void ScrapHand() {
		StopAllCoroutines();
		if (!keepDiscardPile) {
			keepDiscardPile = new GameObject();
			keepDiscardPile.name = "Keep Discard Pile";
			keepDiscardPile.transform.rotation = Quaternion.Euler(0, 180, 0);
		}

		CardBase[] oldCards = GameObject.FindObjectsOfType<CardBase>();

		foreach (CardBase card in oldCards) {
			//keepDiscardPile.transform.position = KeepToScreen();
			//card.DiscardTo(keepDiscardPile);
			Destroy(card.gameObject);
		}
	}

	public void DealHand() {
		// if player is mashing the sleep button
		ScrapHand();
		if (!keep) {
			GameObject g = GameObject.Find("Keep");
			// a second try, if it's been destroyed
			if (!g && !firstDay) {
				return;
			}
		}
		firstDay = false;
		List<CardBase> cards = new List<CardBase>();
		
		foreach (CardSource cardSource in GetCardSources()) {
			cards.AddRange(cardSource.GetCards());
		}

		StartCoroutine(DealCards(cards));
	}

	CardSource[] GetCardSources() {
		return GameObject.FindObjectsOfType<CardSource>()
			.OrderByDescending(x => x.priority)
			.ToArray();
	}

	Vector3 KeepToScreen() {
		if (!keep) {
			GameObject g = GameObject.Find("Keep");
			// a second try, if it's been destroyed
			if (!g) {
				return Vector3.zero;
			}
			keep = g.GetComponent<GameTile>();
		}
		return Camera.main.WorldToScreenPoint(keep.worldPosition);
	}

	IEnumerator DealCards(List<CardBase> cards) {
		foreach (CardBase card in cards) {
			card.transform.position = KeepToScreen();
			card.transform.rotation = Quaternion.Euler(0, 180, 0);
			card.gameObject.SetActive(true);
			yield return new WaitForSeconds(cardInterval);
		}
	}

	public void OnGameBoardChanged() {
		List<CardBase> cards = new List<CardBase>();
		foreach (CardSource source in GetCardSources()) {
			cards.AddRange(source.GetMidRoundCards());
		}
		StartCoroutine(DealCards(cards));
	}
}
