using UnityEngine;
using UnityEngine.UI;

public class BlueprintAction : ActionButton {

	#pragma warning disable 0649
	[Header("Linked Data")]
	[SerializeField] GameTile gameTile;
	[SerializeField] Text tileName;
	[SerializeField] Image tileIcon;
	[SerializeField] Image buttonIcon;
	[SerializeField] Transform tileInfoContainer;
	[SerializeField] Transform resourceContainer;

	[Header("Templates")]
	[SerializeField] GameObject infoLine;
	[SerializeField] GameObject resourceRequirement;
	#pragma warning restore 0649

	override protected void Start() {
		base.Start();
		if (gameTile) Initialize(gameTile);
	}

	public void Initialize(GameTile tile) {
		ClearChildren(tileInfoContainer.transform);
		ClearChildren(resourceContainer.transform);

		tileIcon.sprite = tile.GetDefaultTile().m_DefaultSprite;
		tileIcon.SetNativeSize();
		gameTile = tile;

		buttonIcon.sprite = tile.GetDefaultTile().m_DefaultSprite;
		buttonIcon.SetNativeSize();

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

	protected override void TargetTile(Vector3 tileWorldPosition) {
		PlacementTestResult r = TestPlacement(tileTracker.WorldToBoard(tileWorldPosition));
		ScriptableTile defaultTile = this.gameTile.GetDefaultTile();

		if (!r.valid) {
			ShowActionWarning(r.message, tileWorldPosition, 0);
			tilemapVisuals.HideTilePreview();
		} else {
			HideActionWarning();
			tilemapVisuals.ShowTilePreview(this.gameTile.GetDefaultTile(), r.valid, tileWorldPosition);
		}
	}

	protected override PlacementTestResult TestPlacement(Vector3Int boardPosition) {
		return tileTracker.ValidPlacement(gameTile, boardPosition);
	}

	protected override void ApplyAction(Vector3Int boardPosition) {
		tileTracker.ReplaceTile(boardPosition, this.gameTile.GetDefaultTile());
	}
}
