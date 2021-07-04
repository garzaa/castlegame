using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(TargetLerp))]
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	#pragma warning disable 0649
	[SerializeField] GameTile gameTile;
	[SerializeField] Text tileName;
	[SerializeField] Image tileIcon;
	[SerializeField] Transform tileInfoContainer;
	[SerializeField] Transform resourceContainer;

	[Header("Templates")]
	[SerializeField] GameObject infoLine;
	[SerializeField] GameObject resourceRequirement;
	[SerializeField] InvalidPlacementWarning invalidPlacementWarningTemplate;
	#pragma warning restore 0649

	InvalidPlacementWarning placementWarning;
	CardHand cardHand;
	TargetLerp lerp;
	GameObject handTarget;
	GameObject handPeek;
	bool inHand;
	GameObject boardTarget;
	bool targetingBoard;
	Animator animator;
	TilemapVisuals tilemapVisuals;

	public static Card dragged { get; private set; }
	public static Card hovered { get; private set; }

	void Start() {
		tilemapVisuals = GameObject.FindObjectOfType<TilemapVisuals>();
		cardHand = GameObject.FindObjectOfType<CardHand>();
		animator = GetComponent<Animator>();
		lerp = GetComponent<TargetLerp>();
		ReturnToHand();
		if (gameTile) Initialize(gameTile);
	}

	void OnDestroy() {
		DestroyIfExists(handTarget);
		DestroyIfExists(handPeek);
		if (placementWarning) DestroyIfExists(placementWarning.gameObject);
	}

	void DestroyIfExists(GameObject g) {
		if (g) Destroy(g);
	}

	void ReturnToHand() {
		tilemapVisuals.ClearTilePreview();
		animator.SetTrigger("RestoreImmediate");
		animator.SetBool("PlacePreview", false);
		inHand = true;
		if (!handTarget) {
			handTarget = cardHand.AddHandTarget();
		}
		// so they're stacked nicely, cards need to be in their own object for this unfortunately
		transform.SetSiblingIndex(handTarget.transform.GetSiblingIndex());
		lerp.target = handTarget;
		if (placementWarning) {
			placementWarning.gameObject.SetActive(false);
		}
	}

	void Peek() {
		if (!handPeek) {
			handPeek = new GameObject();
			handPeek.transform.parent = handTarget.transform;
			handPeek.transform.localPosition = new Vector3(0, 129, 0);
		}
		lerp.target = handPeek;

		transform.SetAsLastSibling();
	}

	void UnPeek() {
		lerp.target = handTarget;
		// re-insert into hand order
		transform.SetSiblingIndex(handTarget.transform.GetSiblingIndex());
	}

	public void OnPointerEnter(PointerEventData d) {
		if (Card.dragged && Card.dragged != this) return;
		if (inHand) {
			Peek();
		}
		Card.hovered = this;
	}

	public void OnPointerExit(PointerEventData d) {
		if (inHand) {
			UnPeek();
		}
		Card.hovered = null;
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
		if (!inHand && !targetingBoard) {
			transform.position = Input.mousePosition;
		}
	}

	void PlaceTile() {
		Destroy(this);
	}

	void _TargetTile(Vector3 tileWorldPosition, TileTracker tileTracker) {
		if (!boardTarget) {
			boardTarget = new GameObject();
			boardTarget.transform.parent = dragged.transform.parent;
		}
		float margin = 3f/(float)CameraZoom.GetZoomLevel();
		boardTarget.transform.position = Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.up * margin));
		lerp.target = dragged.boardTarget;
		targetingBoard = true;

		// then hide the card for the tile preview
		animator.SetBool("PlacePreview", true);

		// then run the validator if it's a blueprint card
		// if (dragged is BlueprintTile)
		// then run validator, if result 1 is false, add the message with result 2 above the card, sure
		Tuple<bool, string> placementTest = tileTracker.ValidPlacement(gameTile, tileTracker.WorldToBoard(tileWorldPosition));

		tilemapVisuals.ShowTilePreview(this.gameTile, placementTest.Item1, tileWorldPosition);

		// show/hide invalid warning
		if (!placementTest.Item1) {
			if (!placementWarning) {
				placementWarning = Instantiate(invalidPlacementWarningTemplate, transform.parent);
			}

			// make it fade out from the center if it wasn't active before
			if (!placementWarning.gameObject.activeSelf) {
				placementWarning.transform.position = this.transform.position;
				placementWarning.gameObject.SetActive(true);
			}
			placementWarning.SetInfo(placementTest.Item2, Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.up * margin)));
		} else {
			if (placementWarning) {
				placementWarning.gameObject.SetActive(false);
			}
		}
	}

	public static void TargetTile(Vector3 tileWorldPosition, TileTracker tileTracker) {
		dragged._TargetTile(tileWorldPosition, tileTracker);
	}

	public static void StopTargetingTile() {
		Destroy(dragged.boardTarget);
		dragged.lerp.target = null;
		dragged.targetingBoard = false;
		dragged.animator.SetBool("PlacePreview", false);
		if (dragged.placementWarning) dragged.placementWarning.gameObject.SetActive(false);
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
