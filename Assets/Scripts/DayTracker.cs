using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayTracker : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameEvent dayEndEvent;
	[SerializeField] GameEvent dayStartEvent;
	[SerializeField] AudioResource dayEndSound;

	[SerializeField] GameObject dayUI;
	[SerializeField] GameObject actionContainer;
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
		UpdateActionUI(0);
	}

	void UpdateActionUI(int actionCount) {
		Image[] actions = actionContainer.GetComponentsInChildren<Image>();
		for (int i=0; i<actions.Length; i++) {
			actions[i].enabled = (i < actionCount);
		}
	}

	public void UseAction() {
		actionsToday++;
		totalActions++;
		StopAllCoroutines();
		UpdateActionUI(actionsToday);
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

	public void EndDay(bool silent=false) {
		if (actionsToday == 0) {
			daysWithoutActions++;
		}
		StopAllCoroutines();	
		StartCoroutine(SlowActionReset());
		actionsToday = 0;
		totalDays++;
		dayEndEvent.Raise();
		if (silent) dayEndSound.PlayFrom(this.gameObject);
		AnnounceDay(totalDays);
	}

	IEnumerator SlowActionReset() {
		for (int i=actionsToday; i>=0; i--) {
			yield return new WaitForSeconds(0.2f);
			UpdateActionUI(i);
		}
	}

	public void SleepFor(int days) {
		StartCoroutine(Sleep(days));
	}

	IEnumerator Sleep(int days) {
		EndDay(silent:true);
		if (days > 1) yield return new WaitForSeconds(sleepTime);
		days--;
		if (days > 0) {
			StartCoroutine(Sleep(days));
		} else {
			StartDay();
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
