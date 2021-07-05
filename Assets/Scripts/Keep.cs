using UnityEngine;
using System.Collections.Generic;

public class Keep : CardSource {
	#pragma warning disable 0649
	[SerializeField] ActionCard cutCard;
	#pragma warning restore 0649

	override public List<CardBase> GetCards() {
		List<CardBase> cards = new List<CardBase>();
		cards.Add(MakeCard(cutCard));
		cards.Add(MakeCard(cutCard));
		return cards;
	}

	CardBase MakeCard(ActionCard template) {
		CardBase c = Instantiate(template);
		c.gameObject.SetActive(false);
		return c;
	}
}
