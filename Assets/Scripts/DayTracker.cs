using UnityEngine;

public class DayTracker : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] public GameEvent dayEndEvent;
	[SerializeField] public GameEvent dayStartEvent;
	#pragma warning disable 0649

	const int actionsPerDay = 3;
	int actionsToday = 0;

	int totalDays = 0;
	int totalActions = 0;
	int daysWithoutActions = 0;

	void Start() {
		StartDay();
	}

	public void UseAction() {
		actionsToday++;
		totalActions++;

		if (actionsToday >= actionsPerDay) {
			EndDay();
		}
	}

	public void EndDay() {
		if (actionsToday == 0) {
			
		}
		dayEndEvent.Raise();
		StartDay();
	}

	public void StartDay() {
		actionsToday = 0;
		dayStartEvent.Raise();
	}
}
