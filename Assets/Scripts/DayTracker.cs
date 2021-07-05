using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayTracker : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameEvent dayEndEvent;
	[SerializeField] GameEvent dayStartEvent;
	[SerializeField] AudioResource dayEndSound;

	[SerializeField] GameObject dayUI;
	[SerializeField] Text dayText;
	[SerializeField] GameObject dayAnnouncement;
	#pragma warning disable 0649

	const int actionsPerDay = 3;
	const float sleepTime = 1f;

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
		AnnounceDay(totalDays);
	}

	public void SleepFor(int days) {
		StartCoroutine(Sleep(days));
	}

	IEnumerator Sleep(int days) {
		EndDay();
		if (days > 1) yield return new WaitForSeconds(sleepTime);
		days--;
		if (days > 0) {
			StartCoroutine(Sleep(days));
		}
	}

	void AnnounceDay(int day) {
		GameObject d = Instantiate(dayAnnouncement, dayUI.transform);
		d.gameObject.SetActive(true);
		foreach (Text t in d.GetComponentsInChildren<Text>()) {
			t.text = "DAY "+day;
		}
	}
}
