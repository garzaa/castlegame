using UnityEngine;

public class TutorialBox : MonoBehaviour {
	public bool disableOnStart = true;
	public GameObject nextItem;
	public GameObject nextButton;
	public GameObject toggleButton;
	public GameObject buttonContainer;

	void Start() {
		if (disableOnStart) {
			buttonContainer.SetActive(false);
		}
		if (!nextItem) {
			nextButton.SetActive(false);
		}
	}

	public void Next() {
		if (nextItem) {
			nextItem.SetActive(true);
			nextItem.GetComponent<TutorialBox>().buttonContainer.SetActive(true);
		}
		buttonContainer.SetActive(false);
	}

	public void ToggleBox() {
		buttonContainer.SetActive(!buttonContainer.activeSelf);
	}
}
