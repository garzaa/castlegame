using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFlavorText : MonoBehaviour {
	#pragma warning disable 0649
	[TextArea]
	public string text;

	[SerializeField] Text letterText;
	[SerializeField] GameObject letter;
	[SerializeField] GameObject nextLevelButton;
	[SerializeField] GameObject reloadButton;
	[SerializeField] GameObject closeButton;
	[SerializeField] GameEvent dismissedFirstTime;
	DayTracker dayTracker;
	bool won;
	bool lost;
	bool firedDismissed = false;
	#pragma warning disable 0649

	Levels levels;
	string originalLetterText;
	bool hasSustainCriterion;

	void Start() {
		dayTracker = GameObject.FindObjectOfType<DayTracker>();
		letterText.text = $"Level {GameObject.FindObjectOfType<Levels>().GetLevelNumber()+1}: {SceneManager.GetActiveScene().name}";

		letterText.text += "\n\n"+this.text;

		WinCondition win = GameObject.FindObjectOfType<WinCondition>();
		letterText.text += "\n\n" + win.GetDescription();

		originalLetterText = letterText.text;

		letter.SetActive(true);

		levels = GameObject.FindObjectOfType<Levels>();
		nextLevelButton.SetActive(false);
		reloadButton.SetActive(false);
		hasSustainCriterion = GameObject.FindObjectOfType<SustainCriterion>() != null;
	}

	public void OnDayEnd() {
		if (hasSustainCriterion && !won) {
			letterText.text = originalLetterText;
			int d = dayTracker.GetDaysWithoutActions();
			letterText.text += "\n\nCurrent streak: " + d + (d == 1 ? " day." : " days.");
		}
	}

	public void OnWin() {
		if (lost) return;
		won = true;

		letterText.text = originalLetterText;
		letterText.text += "\n\nComplete!";
		letterText.text += $"\nTook <color='#7a09fa'>{dayTracker.GetTotalDays()}</color> days & <color='#7a09fa'>{dayTracker.GetTotalActions()}</color> actions.";
		letter.SetActive(true);
		if (levels.HasNextLevel()) {
			nextLevelButton.SetActive(true);
			// closeButton.SetActive(false);
		}

	}

	public void OnLose() {
		if (won) return;
		lost = true;

		letterText.text += "\n\n<color='#ea323c'>FAILED.</color>\nYour Keep has fallen into ruin.";
		letter.SetActive(true);
		reloadButton.SetActive(true);
		// closeButton.SetActive(false);
	}
	
	public void ToggleLetter() {
		letter.SetActive(!letter.activeSelf);

		if (!letter.activeSelf && !firedDismissed) {
			dismissedFirstTime.Raise();
			firedDismissed = true;
		}
	}

	public void ReloadLevel() {
		levels.ReloadLevel();
	}

	public void NextLevel() {
		levels.LoadNextLevel();
	}
}

