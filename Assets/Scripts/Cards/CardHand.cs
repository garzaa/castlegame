using UnityEngine;
using UnityEngine.UI;

public class CardHand : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject lerpTargetTemplate;
	[SerializeField] GameObject cardTemplate;
	#pragma warning restore 0649

	HorizontalLayoutGroup layoutGroup;
	Rect container;
	float cardWidth;
	Canvas parentCanvas;
	public bool squishedLastFrame;

	public float childWidth;
	public float handWidth;

	void Start() {
		layoutGroup = GetComponent<HorizontalLayoutGroup>();
		cardWidth = cardTemplate.GetComponent<RectTransform>().rect.width;
		// don't calculate this.width to start because screen size can change
		// also don't grab this.width because it's got left and right properties instead of a width value
		container = GetComponentInParent<RectTransform>().rect;
		parentCanvas = GetComponentInParent<Canvas>();
	}

	public GameObject AddHandTarget() {
		return Instantiate(lerpTargetTemplate, this.transform);
	}

	public void RemoveHandTarget(GameObject target) {
		Destroy(target);
	}

	void Update() {
		childWidth = transform.childCount * cardWidth * parentCanvas.scaleFactor;
		handWidth = container.width - layoutGroup.padding.left - layoutGroup.padding.right;
		// handWidth *= parentCanvas.scaleFactor;
		bool squished = childWidth > handWidth;
		if (squished && !squishedLastFrame) {
			Debug.Log("unsquishing");
			layoutGroup.childControlWidth = true;
			layoutGroup.childForceExpandWidth = true;
			layoutGroup.childScaleWidth = false;

			LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
		} else if (!squished && squishedLastFrame) {
			Debug.Log("squishing");
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
