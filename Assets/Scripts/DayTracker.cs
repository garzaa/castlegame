using UnityEngine;
using UnityEngine.UI;

public class DayTracker : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameEvent dayEndEvent;
	[SerializeField] GameEvent dayStartEvent;
	[SerializeField] AudioResource dayEndSound;

	[SerializeField] GameObject dayUI;
	[SerializeField] Text dayText;
	#pragma warning disable 0649

	const int actionsPerDay = 3;
	int actionsToday = 0;

	int totalDays = 1;
	int totalActions = 0;
	int daysWithoutActions = 0;

	void Start() {
		dayUI.SetActive(true);
		StartDay();
	}

	public void UseAction() {
		actionsToday++;
		totalActions++;
		daysWithoutActions = 0;

		if (actionsToday >= actionsPerDay) {
			EndDay();
			StartDay();
		}
	}

	public void StartDay() {
		dayText.text = "Day "+totalDays;
		dayStartEvent.Raise();
	}

	public void EndDay() {
		if (actionsToday == 0) {
			daysWithoutActions++;
		}
		actionsToday = 0;
		totalDays++;
		dayEndEvent.Raise();
		dayEndSound.PlayFrom(this.gameObject);
	}

	public void SleepFor(int days) {
		// wait, end day, repeat
	}
}
