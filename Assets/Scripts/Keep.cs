using UnityEngine;
using System.Collections.Generic;

public class Keep : CardSource {
	#pragma warning disable 0649
	[SerializeField] ActionCard cutCard;
	#pragma warning restore 0649

	override public List<CardBase> GetCards() {
		List<CardBase> cards = new List<CardBase>();
		CardBase cut = Instantiate(cutCard);
		cut.gameObject.SetActive(false);
		cards.Add(cut);
		return cards;
	}
}
