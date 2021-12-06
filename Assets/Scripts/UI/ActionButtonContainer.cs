using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ActionButtonContainer : MonoBehaviour {
	GameTile keep;
	TileTracker tileTracker;
	ActionTargeter actionTargeter;

	bool firstDay = true;
	bool started = false;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
		actionTargeter = GetComponent<ActionTargeter>();
		started = true;
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
		if (!started) Start();

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

		// maybe make this a game event later
		actionTargeter.OnButtonsFinishCreating();
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
