using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CardDealer : MonoBehaviour {
	GameTile keep;
	GameObject keepDiscardPile;
	TileTracker tileTracker;
	const float cardInterval = 0.1f;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
	}

	public void ScrapHand() {
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
		List<CardBase> cards = new List<CardBase>();
		foreach (CardSource cardSource in GameObject.FindObjectsOfType<CardSource>()) {
			cards.AddRange(cardSource.GetCards());
		}
		StartCoroutine(DealCards(cards));
	}

	Vector3 KeepToScreen() {
		if (!keep) {
			keep = GameObject.Find("Keep").GetComponent<GameTile>();
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
}
