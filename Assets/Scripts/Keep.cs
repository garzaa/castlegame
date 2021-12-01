using UnityEngine;
using System.Collections.Generic;

public class Keep : ButtonSource {
	#pragma warning disable 0649
	[SerializeField] ActionCard cutCard;
	[SerializeField] ActionCard fixCard;
	[SerializeField] EditAction cutButton;
	[SerializeField] EditAction fixButton;
	#pragma warning restore 0649

	public List<CardBase> GetCards() {
		List<CardBase> cards = new List<CardBase>();
		cards.Add(MakeCard(cutCard));
		cards.Add(MakeCard(fixCard));
		return cards;
	}

	CardBase MakeCard(ActionCard template) {
		CardBase c = Instantiate(template);
		c.gameObject.SetActive(false);
		return c;
	}

	override public List<ActionButton> GetButtons() {
		List<ActionButton> buttons = new List<ActionButton>();
		buttons.Add(MakeButton(cutButton));
		buttons.Add(MakeButton(fixButton));
		return buttons;
	}

	ActionButton MakeButton(EditAction template) {
		ActionButton a = Instantiate(template);
		return a;
	}
}
