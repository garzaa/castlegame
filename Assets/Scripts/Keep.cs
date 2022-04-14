using UnityEngine;
using System.Collections.Generic;

public class Keep : ButtonSource {
	#pragma warning disable 0649
	[SerializeField] EditAction cutButton;
	[SerializeField] EditAction fixButton;
	#pragma warning restore 0649

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
