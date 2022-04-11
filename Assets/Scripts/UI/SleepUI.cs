using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SleepUI : MonoBehaviour {

	const float maxVal = 30;
	const float clickDelta = 1f/maxVal;
	const float velocityCutoff = 3f;

	#pragma warning disable 0649
	[SerializeField] Image buttonGraphic;
	[SerializeField] AudioResource sliderClick;
	[SerializeField] AudioResource clickNoise;
	[SerializeField] Sprite moon;
	[SerializeField] Sprite sun;
	#pragma warning restore 0649

	ScrollRect scrollRect;
	DayTracker dayTracker;

	Vector2 sleepSliderPosition;
	float lastClickPosition = 0;
	float currentDelta;
	bool sleeping = false;

	void Awake() {
		scrollRect = GetComponentInChildren<ScrollRect>();
	}

	void Start() {
		dayTracker = GameObject.FindObjectOfType<DayTracker>();
	}

	public void OnSleepButtonClick() {
		if (!sleeping) {
			// slider defaults to 0
			int d = Mathf.Max(GetDays(), 1);
			clickNoise.PlayFrom(buttonGraphic.gameObject);
			dayTracker.SleepFor(d);
			buttonGraphic.sprite = sun;
			sleeping = true;
		} else {
			dayTracker.Wake();
			sleeping = false;
			buttonGraphic.sprite = moon;
		}
	}

	public void OnDayEnd() {
		if (sleeping) {
			sliderClick.PlayFrom(this.gameObject);
			clickNoise.PlayFrom(buttonGraphic.gameObject);
		}
	}

	int GetDays() {
		return Mathf.RoundToInt(scrollRect.normalizedPosition.x * maxVal);
	}

	void Update() {
		if (sleeping) {
			int daysLeft = dayTracker.daysLeftToSleep;
			if (daysLeft == 0) {
				sleeping = false;
				buttonGraphic.sprite = moon;
			}
			scrollRect.horizontal = false;
			Vector2 rectPos = scrollRect.normalizedPosition;
			// convert from days left (int) to position (float)
			rectPos.x = ((float) daysLeft-1) / maxVal;
			scrollRect.normalizedPosition = rectPos;
		} else {
			scrollRect.horizontal = true;
		}
	}

	public void OnScrollViewMove(Vector2 pos) {
		if (sleeping) return;
		sleepSliderPosition = pos;
		currentDelta = Mathf.Abs(pos.x - lastClickPosition);
		if (currentDelta > clickDelta) {
			sliderClick.PlayFrom(this.gameObject);
			lastClickPosition = pos.x;
		}
		if (Mathf.Abs(scrollRect.velocity.x) < velocityCutoff) {
			bool movingUp = scrollRect.velocity.x < 0f;
			float xPos = scrollRect.normalizedPosition.x * maxVal;
			int days = movingUp ? Mathf.CeilToInt(xPos) : Mathf.FloorToInt(xPos);
			float newXPos = (float) days / maxVal;
			scrollRect.normalizedPosition = new Vector2(newXPos, scrollRect.normalizedPosition.y);
			scrollRect.velocity = Vector2.zero;
			sliderClick.PlayFrom(this.gameObject);
		}
	}
}
