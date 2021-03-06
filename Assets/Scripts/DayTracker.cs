using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DayTracker : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameEvent dayEndEvent;
	[SerializeField] GameEvent dayStartEvent;
	[SerializeField] AudioResource dayEndSound;
	[SerializeField] AudioResource winSound;
	[SerializeField] AudioResource loseSound;

	[SerializeField] GameObject dayUI;
	[SerializeField] GameEvent winEvent;
	[SerializeField] GameObject actionContainer;
	[SerializeField] Text dayText;
	[SerializeField] GameObject dayAnnouncement;
	[SerializeField] Text totalActionsText;
	#pragma warning disable 0649

	const int actionsPerDay = 3;
	const float sleepTime = 1f;

	bool gameOver = false;
	bool wonLevel = false;
	bool raisedWon = false;
	bool sleeping = false;

	int actionsToday = 0;
	int totalDays = 1;
	int totalActions = 0;
	int daysWithoutActions = 0;
	public int daysLeftToSleep {
		get; private set;
	}

	TileTracker tileTracker;
	WinCondition[] winConditions;
	List<GameObject> actions = new List<GameObject>();

	void Awake() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
		winConditions = GameObject.FindObjectsOfType<WinCondition>();
		
		foreach (Transform child in actionContainer.transform) {
			actions.Add(child.GetChild(0).gameObject);
		}
	}

	void Start() {
		dayUI.SetActive(true);
		StartDay();
		UpdateActionUI(0);
		UpdateDayText();
	}

	void UpdateActionUI(int actionCount) {
		for (int i=0; i<actions.Count; i++) {
			actions[i].SetActive(i < actionCount);
		}
		totalActionsText.text = totalActions + " total";
	}

	void UpdateDayText() {
		dayText.text = "Day "+totalDays;
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
		dayStartEvent.Raise();
	}

	public void EndDay(bool silent=false) {
		if (actionsToday == 0) {
			daysWithoutActions++;
		}
		CheckWinConditions();
		StopCoroutine("SlowActionReset");	
		StartCoroutine(SlowActionReset());
		actionsToday = 0;
		totalDays++;
		UpdateDayText();
		dayEndEvent.Raise();
		if (!silent) {
			dayEndSound.PlayFrom(this.gameObject);
		}
		AnnounceDay(totalDays);
	}

	IEnumerator SlowActionReset() {
		for (int i=actionsToday; i>=0; i--) {
			yield return new WaitForSeconds(0.2f);
			UpdateActionUI(i);
		}
	}

	public void SleepFor(int days) {
		if (days == 1) {
			EndDay();
			StartDay();
			return;
		}
		sleeping = true;
		daysLeftToSleep = days;
		StartCoroutine(Sleep(days));
	}

	IEnumerator Sleep(int days) {
		EndDay(silent:true);
		if (days > 0) yield return new WaitForSeconds(sleepTime);
		days--;
		daysLeftToSleep = days;
		if (days > 0 && (!wonLevel || raisedWon) && sleeping) {
			StartCoroutine(Sleep(days));
		} else {
			StartDay();
		}
	}

	public void Wake() {
		daysLeftToSleep = 0;
		sleeping = false;
	}

	void AnnounceDay(int day) {
		GameObject d = Instantiate(dayAnnouncement, dayUI.transform);
		d.gameObject.SetActive(true);
		foreach (Text t in d.GetComponentsInChildren<Text>()) {
			t.text = "DAY "+day;
		}
	}

	public void CheckWinConditions() {
		if (wonLevel || gameOver) return;
		WinCondition won = null;
		foreach (WinCondition c in winConditions) {
			if (c.Satisfied(tileTracker)) {
				won = c;
				break;
			}
		}
		if (won) {
			winSound.PlayFrom(this.gameObject);
			winEvent.Raise();
			raisedWon = true;
			wonLevel = true;
			Wake();
		}
	}

	public void OnLose() {
		loseSound.PlayFrom(this.gameObject);
		gameOver = true;
		Wake();
	}

	public int GetTotalActions() {
		return totalActions;
	}

	public int GetTotalDays() {
		return totalDays;
	}

	public int GetDaysWithoutActions() {
		return daysWithoutActions;
	}
}
