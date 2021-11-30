using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ActionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	#pragma warning disable 0649
	[Header("Settings")]
	[SerializeField] protected Tile actionIcon;
	[SerializeField] bool useAction = true;
	[SerializeField] Sprite hoveredSprite;
	[SerializeField] Sprite activeSprite;

	[Header("Linked Data")]
	[SerializeField] AudioResource hoverSound;
	[SerializeField] AudioResource clickSound;
	[SerializeField] AudioResource placeSound;
	[SerializeField] Image border;
	[SerializeField] GameObject infoCard;

	[Header("Templates")]
	[SerializeField] InvalidPlacementWarning invalidPlacementWarningTemplate;
	#pragma warning restore 0649

	Animator animator;
	DayTracker dayTracker;
	ActionTargeter actionTargeter;

	protected TileTracker tileTracker;
	protected InvalidPlacementWarning actionWarning;
	protected TilemapVisuals tilemapVisuals;

	protected virtual void Start() {
		animator = GetComponent<Animator>();
		dayTracker = GameObject.FindObjectOfType<DayTracker>();
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
		actionTargeter = GameObject.FindObjectOfType<ActionTargeter>();
		tilemapVisuals = GameObject.FindObjectOfType<TilemapVisuals>();

		GetComponent<RectTransform>().SetParent(GameObject.FindObjectOfType<ActionButtonContainer>().transform, worldPositionStays: false);
		foreach (Image i in GetComponentsInChildren<Image>()) {
			i.SetNativeSize();
		}

		// TODO: spawn and populate an info card with everything

		infoCard.SetActive(false);
	}

	public void OnGridClick(Vector3Int boardPosition) {
		actionTargeter.ClearArmedAction();
		ApplyAction(boardPosition);
	}

	public bool TryToApplyAction(Vector3Int boardPosition) {
		if (TestPlacement(boardPosition).valid) {
			ApplyAction(boardPosition);
			OnApply();
			return true;
		} else {
			return false;
		}
	}

	void OnApply() {
		if (useAction) dayTracker.UseAction();
		if (placeSound) placeSound.PlayFrom(this.gameObject);
	}

	protected virtual PlacementTestResult TestPlacement(Vector3Int boardPosition) {
		throw new System.NotImplementedException();
	}

	protected virtual void ApplyAction(Vector3Int boardPosition) {
		throw new System.NotImplementedException();
	}
	
	protected virtual void TargetTile(Vector3 tileWorldPosition) {
		throw new System.NotImplementedException();
	}

	public void OnTileHover(Vector3 tileWorldPosition) {
		TargetTile(tileWorldPosition);
	}

	public void OnPointerEnter(PointerEventData data) {
		if (IsArmed()) return;
		hoverSound.PlayFrom(this.gameObject);
		border.sprite = hoveredSprite;
		border.enabled = true;
		infoCard.SetActive(true);

		// TODO: show the info card
	}

	public void OnPointerExit(PointerEventData data) {
		infoCard.SetActive(false);
		if (IsArmed()) return;
		border.enabled = false;
		border.sprite = null;
	}

	public void OnPointerClick(PointerEventData data) {
		if (IsArmed()) return;
		Arm();
		border.sprite = activeSprite;
		border.enabled = true;
	}

	void Arm() {
		clickSound.PlayFrom(this.gameObject);
		actionTargeter.SetArmedAction(this);
		border.sprite = activeSprite;
	}

	public void Disarm() {
		border.sprite = null;
		border.enabled = false;
		infoCard.SetActive(false);
		HideActionWarning();
	}

	bool IsArmed() {
		return actionTargeter.GetArmedAction() == this;
	}

	public Tile GetActionIcon() {
		return actionIcon;
	}

	protected void ShowActionWarning(string message, Vector3 tileWorldPosition, float margin) {
		if (!actionWarning) {
			actionWarning = Instantiate(invalidPlacementWarningTemplate, actionTargeter.GetErrorContainer());
		}

		// if no message, then don't show anything (mouse out of board bounds)
		if (String.IsNullOrEmpty(message)) {
			actionWarning.gameObject.SetActive(false);
			return;
		}

		// make it fade out from the center if it wasn't active before
		if (!actionWarning.gameObject.activeSelf) {
			actionWarning.transform.position = Camera.main.WorldToScreenPoint(tileWorldPosition);
			actionWarning.gameObject.SetActive(true);
		}
		actionWarning.SetInfo(message, Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.down / 2f)));
	}

	protected void HideActionWarning() {
		if (actionWarning) {
			actionWarning.gameObject.SetActive(false);
		}
	}

	protected void ClearChildren(Transform t) {
		foreach (Transform child in t) {
			GameObject.Destroy(child.gameObject);
		}
	}

	protected void RebuildUI() {
		foreach (LayoutGroup g in GetComponentsInChildren<LayoutGroup>()) {
			LayoutRebuilder.ForceRebuildLayoutImmediate(g.GetComponent<RectTransform>());
		}
	}
}
