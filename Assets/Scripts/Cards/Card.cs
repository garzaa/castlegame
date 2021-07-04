using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(TargetLerp))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	#pragma warning disable 0649
	[SerializeField] Text tileName;
	[SerializeField] Image tileIcon;
	[SerializeField] Transform tileInfoContainer;
	[SerializeField] Transform resourceContainer;

	[Header("Templates")]
	[SerializeField] GameObject infoLine;
	[SerializeField] GameObject resourceRequirement;
	#pragma warning restore 0649

	CardHand cardHand;
	TargetLerp lerp;
	GameObject handTarget;
	GameObject handPeek;
	bool inHand;

	public static Card dragged { get; private set; }

	void Start() {
		cardHand = GameObject.FindObjectOfType<CardHand>();
		lerp = GetComponent<TargetLerp>();
		ReturnToHand();
	}

	void ReturnToHand() {
		if (inHand) return;

		inHand = true;
		if (!handTarget) {
			handTarget = cardHand.AddHandTarget();
		}
		// so they're stacked nicely, cards need to be in their own object for this unfortunately
		transform.SetSiblingIndex(handTarget.transform.GetSiblingIndex());
		lerp.target = handTarget;
	}

	void Peek() {
		if (!handPeek) {
			handPeek = Instantiate(new GameObject(), handTarget.transform);
			handPeek.transform.localPosition = new Vector3(0, 129, 0);
		}
		lerp.target = handPeek;

		// re-insert into hand order
		transform.SetAsLastSibling();
	}

	void UnPeek() {
		lerp.target = handTarget;
		transform.SetSiblingIndex(handTarget.transform.GetSiblingIndex());
	}

	public void OnPointerEnter(PointerEventData d) {
		if (inHand) {
			Peek();
		}
	}

	public void OnPointerExit(PointerEventData d) {
		if (inHand) {
			UnPeek();
		}
	}

	public void OnPointerDown(PointerEventData d) {
		inHand = false;
		Card.dragged = this;
	}

	public void OnPointerUp(PointerEventData d) {
		Card.dragged = null;
		ReturnToHand();
	}

	void LateUpdate() {
		if (!inHand) {
			transform.position = Input.mousePosition;
		}
	}

	public void Initialize(GameTile tile) {
		ClearChildren(tileInfoContainer.transform);
		ClearChildren(resourceContainer.transform);

		tileIcon.sprite = tile.GetDefaultTile().m_DefaultSprite;
		tileIcon.SetNativeSize();

		tileName.text = tile.name;

		foreach (TileRequiredResource resourceList in tile.GetComponents<TileRequiredResource>()) {
			foreach (ResourceAmount resource in resourceList.resources) {
				GameObject g = Instantiate(resourceRequirement, resourceContainer);
				g.GetComponentInChildren<Text>().text = resource.amount.ToString();
				g.GetComponentInChildren<Image>().sprite = resource.resource.icon;
			}
		}

		foreach (ICardStat i in tile.GetComponents<ICardStat>()) {
			string s = i.Stat();
			if (string.IsNullOrEmpty(s)) continue;
			GameObject g = Instantiate(infoLine, tileInfoContainer);
			g.GetComponent<Text>().text = i.Stat();
		}

		RebuildUI();
	}

	void RebuildUI() {
		foreach (LayoutGroup g in GetComponentsInChildren<LayoutGroup>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(g.GetComponent<RectTransform>());
		}
	}

	void ClearChildren(Transform t) {
		foreach (Transform child in t) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
