using UnityEngine;
using UnityEngine.UI;

public class CardHand : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject lerpTargetTemplate;
	[SerializeField] GameObject cardTemplate;
	#pragma warning restore 0649

	HorizontalLayoutGroup layoutGroup;
	Rect thisRect;
	float cardWidth;
	bool squishedLastFrame;

	void Start() {
		layoutGroup = GetComponent<HorizontalLayoutGroup>();
		cardWidth = cardTemplate.GetComponent<RectTransform>().rect.width;
		// don't calculate this.width to start because screen size can change
		thisRect = GetComponent<RectTransform>().rect;
	}

	public GameObject AddHandTarget() {
		return Instantiate(lerpTargetTemplate, this.transform);
	}

	public void RemoveHandTarget(GameObject target) {
		Destroy(target);
	}

	void Update() {
		bool squished = (transform.childCount * cardWidth) > (thisRect.width - layoutGroup.padding.left - layoutGroup.padding.right);
		if (squished && !squishedLastFrame) {
			layoutGroup.childControlWidth = true;
			layoutGroup.childForceExpandWidth = true;
			layoutGroup.childScaleWidth = false;

			LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
		} else if (!squished && squishedLastFrame) {
			layoutGroup.childControlWidth = false;
			layoutGroup.childForceExpandWidth = false;
			layoutGroup.childScaleWidth = true;

			foreach (RectTransform cardChild in GetComponent<RectTransform>()) {
				Vector2 size = cardChild.sizeDelta;
				size.x = cardWidth;
				cardChild.GetComponent<RectTransform>().sizeDelta = size;
			}

			LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
		}

		squishedLastFrame = squished;
	}
}
