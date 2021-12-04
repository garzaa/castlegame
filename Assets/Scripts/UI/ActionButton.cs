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
	[SerializeField] Text letter;

	[Header("Templates")]
	[SerializeField] InvalidPlacementWarning invalidPlacementWarningTemplate;
	#pragma warning restore 0649

	Animator animator;
	DayTracker dayTracker;
	ActionTargeter actionTargeter;
	Animator infoCardAnimator;
	Animator buttonAnimator;

	protected TileTracker tileTracker;
	protected InvalidPlacementWarning actionWarning;
	protected TilemapVisuals tilemapVisuals;

	const string letters = "qwertyuiopasdfghjkl";
	string keyName;
	bool keyPressed;

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

		infoCardAnimator = infoCard.GetComponent<Animator>();
		HideInfoCard();

		buttonAnimator = GetComponent<Animator>();

		keyName = letters[transform.GetSiblingIndex()].ToString();
		letter.text = keyName.ToUpper();
	}

	void Update() {
		keyPressed = Input.GetKey(keyName);
		if (Input.GetKeyDown(keyName)) {
			if (IsArmed()) {
				actionTargeter.ClearArmedAction();
				return;
			}
			MouseEnter();
		}
		if (Input.GetKeyUp(keyName)) {
			MouseClick();
		}
	}

	void HideInfoCard() {
		infoCardAnimator.SetBool("PlacePreview", true);
	}

	void ShowInfoCard() {
		infoCardAnimator.SetBool("PlacePreview", false);
	}

	void ToggleInfoCard() {
		infoCardAnimator.SetBool("PlacePreview", !infoCardAnimator.GetBool("PlacePreview"));
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
		ShowInfoCard();
		if (keyPressed) return;
		MouseEnter();
	}

	public void OnPointerExit(PointerEventData data) {
		HideInfoCard();
		if (keyPressed) return;
		MouseExit();
	}

	public void OnPointerClick(PointerEventData data) {
		MouseClick();
	}

	public void MouseEnter() {
		hoverSound.PlayFrom(this.gameObject);
		border.sprite = hoveredSprite;
		border.enabled = true;
	}

	public void MouseExit() {
		HideInfoCard();
		if (IsArmed()) return;
		border.enabled = false;
		border.sprite = null;
	}

	public void MouseClick() {
		if (IsArmed()) return;
		Arm();
		border.sprite = activeSprite;
		border.enabled = true;
	}

	void Arm() {
		clickSound.PlayFrom(this.gameObject);
		actionTargeter.SetArmedAction(this);
		border.sprite = activeSprite;
		animator.SetBool("Armed", true);
	}

	public void Disarm() {
		border.sprite = null;
		border.enabled = false;
		HideInfoCard();
		HideActionWarning();
		animator.SetBool("Armed", false);
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
			actionWarning.transform.SetParent(tilemapVisuals.GetDoubleScaleCanvas().transform, worldPositionStays: false);
			foreach (Image i in GetComponentsInChildren<Image>()) {
				// to deal with the above line
				i.SetNativeSize();
			}
		}

		// if no message, then don't show anything (mouse out of board bounds)
		if (String.IsNullOrEmpty(message)) {
			actionWarning.gameObject.SetActive(false);
			return;
		}

		if (!actionWarning.gameObject.activeSelf) {
			actionWarning.transform.position = Camera.main.WorldToScreenPoint(tileWorldPosition);
			actionWarning.gameObject.SetActive(true);
		}
		actionWarning.SetInfo(message, Camera.main.WorldToScreenPoint(tileWorldPosition + (Vector3.down / 2f)));
	}

	public void HideActionWarning() {
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

	public void SetTransformParent(Transform p) {
		GetComponent<RectTransform>().SetParent(p, worldPositionStays:false);
		foreach (Image i in GetComponentsInChildren<Image>()) {
			// to deal with the above line
			i.SetNativeSize();
		}
	}

	void OnDestroy() {
		if (actionWarning) GameObject.Destroy(actionWarning);
	}
}
