using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CardBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {
	#pragma warning disable 0649
	[Header("Settings")]
	public bool useAction = true;

	[Header("Linked Data")]
	[SerializeField] AudioResource peekSound;
	[SerializeField] AudioResource clickSound;

	[Header("Templates")]
	[SerializeField] InvalidPlacementWarning invalidPlacementWarningTemplate;
	#pragma warning restore 0649


	protected InvalidPlacementWarning placementWarning;
	protected CardHand cardHand;
	protected TargetLerp lerp;
	protected GameObject handTarget;
	protected GameObject handPeek;
	protected bool inHand;
	protected GameObject boardTarget;
	protected bool targetingBoard;
	protected Animator animator;
	protected TilemapVisuals tilemapVisuals;
	protected Tuple<bool, string> placementTest;
	protected TileTracker tileTracker;
	DayTracker dayTracker;

	public static CardBase dragged { get; private set; }
	public static CardBase hovered { get; private set; }

	protected virtual void Start() {
		tilemapVisuals = GameObject.FindObjectOfType<TilemapVisuals>();
		cardHand = GameObject.FindObjectOfType<CardHand>();
		animator = GetComponent<Animator>();
		dayTracker = GameObject.FindObjectOfType<DayTracker>();
		lerp = GetComponent<TargetLerp>();
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
		ReturnToHand();
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
		peekSound.PlayFrom(this.gameObject);
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
		clickSound.PlayFrom(this.gameObject);
		Card.dragged = this;
	}

	public void OnPointerUp(PointerEventData d) {
		if (targetingBoard && placementTest.Item1) {
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(d.position);
			mouseWorldPos.z = 0;
			if (useAction) dayTracker.UseAction();
			OnDrop(tileTracker.WorldToBoard(mouseWorldPos));
			tilemapVisuals.ClearTilePreview();
		} else {
			Card.dragged = null;
			ReturnToHand();
		}
	}

	void LateUpdate() {
		if (!inHand && !targetingBoard) {
			transform.position = Input.mousePosition;
		}
	}

	protected virtual void OnDrop(Vector3Int boardPosition) {
		throw new System.NotImplementedException();
	}

	virtual protected void _TargetTile(Vector3 tileWorldPosition) {
		throw new System.NotImplementedException();
	}

	public static void TargetTile(Vector3 tileWorldPosition) {
		dragged._TargetTile(tileWorldPosition);
	}

	protected void ShowPlaceWarning(string message, Vector3 tileWorldPosition, float margin) {
		if (!placementWarning) {
			placementWarning = Instantiate(invalidPlacementWarningTemplate, transform.parent);
		}

		// make it fade out from the center if it wasn't active before
		if (!placementWarning.gameObject.activeSelf) {
			placementWarning.transform.position = this.transform.position;
			placementWarning.gameObject.SetActive(true);
		}
		placementWarning.SetInfo(placementTest.Item2, Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.up * margin)));
	}

	protected void HidePlacementWarning() {
		if (placementWarning) {
			placementWarning.gameObject.SetActive(false);
		}
	}

	public static void StopTargetingTile() {
		Destroy(dragged.boardTarget);
		dragged.lerp.target = null;
		dragged.targetingBoard = false;
		dragged.animator.SetBool("PlacePreview", false);
		if (dragged.placementWarning) dragged.placementWarning.gameObject.SetActive(false);
	}

	protected void RebuildUI() {
		foreach (LayoutGroup g in GetComponentsInChildren<LayoutGroup>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(g.GetComponent<RectTransform>());
		}
	}

	protected void ClearChildren(Transform t) {
		foreach (Transform child in t) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
