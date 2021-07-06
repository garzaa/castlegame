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
	DayTracker dayTracker;
	bool won;
	bool lost;
	#pragma warning disable 0649

	Levels levels;

	void Start() {
		letterText.text = $"Level {GameObject.FindObjectOfType<Levels>().GetLevelNumber()+1}: {SceneManager.GetActiveScene().name}";

		letterText.text += "\n\n"+this.text;

		WinCondition win = GameObject.FindObjectOfType<WinCondition>();
		letterText.text += "\n\n" + win.GetDescription();

		letter.SetActive(true);

		levels = GameObject.FindObjectOfType<Levels>();
		nextLevelButton.SetActive(false);
		reloadButton.SetActive(false);
	}

	public void OnWin() {
		if (lost) return;
		won = true;

		DayTracker dayTracker = GameObject.FindObjectOfType<DayTracker>();
		letterText.text += "\n\nComplete!";
		letterText.text += $"\nTook <color='#7a09fa'>{dayTracker.GetTotalDays()}</color> days & <color='#7a09fa'>{dayTracker.GetTotalActions()}</color> actions.";
		letter.SetActive(true);
		if (levels.HasNextLevel()) nextLevelButton.SetActive(true);
	}

	public void OnLose() {
		if (won) return;
		lost = true;

		letterText.text += "\n\n<color='#ea323c'>FAILED.</color>\nYour Keep has fallen into ruin.";
		letter.SetActive(true);
		reloadButton.SetActive(true);
	}
	
	public void ToggleLetter() {
		letter.SetActive(!letter.activeSelf);
	}

	public void ReloadLevel() {
		levels.ReloadLevel();
	}

	public void NextLevel() {
		levels.LoadNextLevel();
	}
}

