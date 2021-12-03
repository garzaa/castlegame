using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ActionButtonContainer : MonoBehaviour {
	GameTile keep;
	TileTracker tileTracker;

	bool firstDay = true;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
	}

	public void ScrapButtons() {
		ActionButton[] oldButtons = GameObject.FindObjectsOfType<ActionButton>();
		foreach (ActionButton b in oldButtons) {
			// destroying happens at the end of the frame, but we want them out of the hierarchy
			// so the keycodes can be generated properly
			b.GetComponent<RectTransform>().SetParent(null);
			Destroy(b.gameObject);
		}
	}

	public void AddButtons() {
		ScrapButtons();

		if (!GameObject.Find("Keep") && !firstDay) {
			return;
		}

		firstDay = false;
		List<ActionButton> buttons = new List<ActionButton>();

		foreach (ButtonSource buttonSource in GetButtonSources()) {
			buttons.AddRange(buttonSource.GetButtons());
		}

		foreach (ActionButton button in buttons) {
			button.SetTransformParent(this.transform);
		}
	}

	ButtonSource[] GetButtonSources() {
		return GameObject.FindObjectsOfType<ButtonSource>()
			.OrderBy(x => x.priority)
			.ToArray();
	}

	public void OnGameBoardChanged() {
		List<ActionButton> cards = new List<ActionButton>();
		foreach (ButtonSource source in GetButtonSources()) {
			cards.AddRange(source.GetMidRoundButtons());
		}
	}
}
