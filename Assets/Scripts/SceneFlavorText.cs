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
	[SerializeField] Text objectiveText;
	#pragma warning disable 0649

	DayTracker dayTracker;
	bool won;
	bool lost;
	bool firedDismissed = false;

	Levels levels;
	string originalLetterText;
	bool hasSustainCriterion;
	GameObject maskContainer;
	Animator animator;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		dayTracker = GameObject.FindObjectOfType<DayTracker>();
		letterText.text = $"Level {GameObject.FindObjectOfType<Levels>().GetLevelNumber()+1}: {SceneManager.GetActiveScene().name}";

		letterText.text += "\n\n"+this.text;

		WinCondition win = GameObject.FindObjectOfType<WinCondition>();
		letterText.text += "\n\n" + "<color=#134c4c>" + win.GetDescription() + "</color>";

		originalLetterText = letterText.text;

		letter.SetActive(true);

		levels = GameObject.FindObjectOfType<Levels>();
		nextLevelButton.SetActive(false);
		reloadButton.SetActive(false);
		hasSustainCriterion = GameObject.FindObjectOfType<SustainCriterion>() != null;
		maskContainer = letter.transform.GetChild(0).gameObject;
		objectiveText.text = win.GetDescription();
	}

	public void OnDayEnd() {
		if (hasSustainCriterion && !won && !lost) {
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
		int d = dayTracker.GetTotalDays();
		int a = dayTracker.GetTotalActions();
		letterText.text += $"\nTook <color='#7a09fa'>{d}</color> day" + (d > 1 ? "s" : "") + " & ";
		letterText.text += $"<color='#7a09fa'>{a}</color> action"+ (a > 1 ? "s" : "") +".";
		OpenLetterFromClick();
		if (levels.HasNextLevel()) {
			nextLevelButton.SetActive(true);
			// closeButton.SetActive(false);
		}

	}

	public void OnLose() {
		if (won) return;
		lost = true;

		letterText.text += "\n\n<color='#ea323c'>FAILED.</color>\nYour Keep has fallen into ruin.";
		OpenLetterFromClick();
		reloadButton.SetActive(true);
		// closeButton.SetActive(false);
	}

	public void CloseLetterFromClick() {
		letter.SetActive(false);
		animator.SetBool("LetterActive", false);

		if (!firedDismissed) {
			dismissedFirstTime.Raise();
			firedDismissed = true;
		}
	}
	
	public void OpenLetterFromClick() {
		animator.SetBool("LetterActive", true);
		letter.SetActive(true);
		maskContainer.SetActive(true);
	}

	public void ReloadLevel() {
		levels.ReloadLevel();
	}

	public void NextLevel() {
		levels.LoadNextLevel();
	}
}

