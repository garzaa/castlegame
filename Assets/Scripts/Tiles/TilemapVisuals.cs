using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TilemapVisuals : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject legendTemplate;
	[SerializeField] Tile highlightTile;
	[SerializeField] Tilemap highlightTilemapTemplate;
	[SerializeField] Tile homeTile;
	[SerializeField] Tile clickedTile;
	[SerializeField] Canvas doubleScaleCanvas; // this gets overridden with the initialized version of itself...is that ok??
	[SerializeField] TileInfo infoBubbleTemplate;
	[SerializeField] GameObject repairEffect;
	[SerializeField] GameObject decayEffect;
	#pragma warning restore 0649

	CommandInput console;
	Tilemap tilemap;
	Vector3Int origin;
	Tilemap highlightTilemap;
	Tilemap iconTilemap;
	Tilemap clickedTilemap;
	Tilemap previewTilemap;
	Tilemap singleIconTilemap;
	Vector3 mouseWorldPos;
	Vector3Int gridMousePos;
	TileTracker tracker;

	GameObject currentInfoBubble;
	bool showingTileVisuals;
	Vector3Int currentSelectedGridPosition;
	Vector3Int targetedTile;

	void Awake() {
		tilemap = GetComponent<Tilemap>();
		tilemap.CompressBounds();
	}

	void Start() {
		CreateHighlightTilemap();
		CreateIconTilemap();
		CreateSelectedTilemap();
		CreatePreviewTilemap();
		CreateSingleIconTilemap();
		CreateDoubleScaleCanvas();
		console = GameObject.FindObjectOfType<CommandInput>();
		tracker = GameObject.FindObjectOfType<TileTracker>();
		origin = tilemap.cellBounds.min;

		for (int i=0; i<tilemap.cellBounds.size.x; i++) {
			AddLetterLegend(i);
		}

		for (int i=0; i<tilemap.cellBounds.size.y; i++) {
			AddNumberLegend(i);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			HideInfoBubble();
		}
	}

	void AddLetterLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.right*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = TileTracker.letters[idx].ToString().ToUpper();
	}

	void AddNumberLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.up*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = (idx+1).ToString();
	}

	void CreateDoubleScaleCanvas() {
		doubleScaleCanvas = Instantiate(doubleScaleCanvas, this.transform);
	}
	
	void CreateHighlightTilemap() {
		highlightTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		highlightTilemap.GetComponent<TilemapRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateIconTilemap() {
		iconTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		iconTilemap.GetComponent<TilemapRenderer>().sortingOrder = highlightTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateSelectedTilemap() {
		clickedTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		clickedTilemap.GetComponent<TilemapRenderer>().sortingOrder = iconTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreatePreviewTilemap() {
		previewTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		previewTilemap.GetComponent<TilemapRenderer>().sortingOrder = clickedTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateSingleIconTilemap() {
		singleIconTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		singleIconTilemap.GetComponent<TilemapRenderer>().sortingOrder = previewTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	public void HighlightTile(Vector3Int gridPos) {
		if (!tilemap.cellBounds.Contains(gridPos)) {
			return;
		}
		highlightTilemap.SetTile(gridPos, highlightTile);
	}

	public void HighlightTiles(IEnumerable<GameTile> tiles) {
		foreach (GameTile tile in tiles) {
			highlightTilemap.SetTile(tile.gridPosition, highlightTile);
		}
	}

	void OnMouseOver() {
		mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		gridMousePos = highlightTilemap.WorldToCell(mouseWorldPos);
		gridMousePos.z = 0;

		if (!tilemap.cellBounds.Contains(gridMousePos)) {
			OnMouseExit();
		}

		if (gridMousePos == targetedTile) return;
		targetedTile = gridMousePos;
		if (CardBase.dragged) {
			CardBase.TargetTile(tilemap.CellToWorld(targetedTile));
			ShowSingleIcon(CardBase.dragged.GetActionIcon(), gridMousePos);
		} else if (CardBase.hovered) {
			// don't track mouse position if the player is just looking
			highlightTilemap.ClearAllTiles();
			return;
		}
	
		highlightTilemap.ClearAllTiles();
		HighlightTile(gridMousePos);
	}

	void OnMouseExit() {
		if (CardBase.dragged) {
			CardBase.StopTargetingTile();
			ClearTilePreview();
		}
	}

	void OnMouseDown() {
		// this will fire if a card is being held or peeked over the board
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}
		// don't return inside the function because the tilemap calls that to refresh on a day end
		if (gridMousePos == currentSelectedGridPosition) return;
		OnTileClick(gridMousePos);
	}

	void OnTileClick(Vector3Int gridPos, bool silent=false) {
		GameTile gameTile = tracker.GetTileNoRedirect(tracker.CellToBoard(gridPos));
		if (gameTile == null) return;
		currentSelectedGridPosition = gridPos;
		if (!silent) gameTile.PlayQuerySound();
		DisplayTileVisuals(gameTile);
		ShowInfoBubble(gameTile);
	}

	public void ShowTilePreview(ScriptableTile tile, bool valid, Vector3 tileWorldPosition) {
		ClearTilePreview();
		previewTilemap.SetTile(previewTilemap.WorldToCell(tileWorldPosition), tile);
		// TODO: if valid, render it and the associated effects on the preview tilemap
		// if not, just render it and the card info will take care of the rest
	}

	public void ClearTilePreview() {
		// alleviate race condition on start
		if (previewTilemap) {
			previewTilemap.ClearAllTiles();
		}
		if (singleIconTilemap) {
			singleIconTilemap.ClearAllTiles();
		}
	}

	public void ShowTileEffect(GameObject effectUI, Vector3 tileWorldPosition) {
		WorldPointCanvas w = Instantiate(effectUI, doubleScaleCanvas.transform).GetComponent<WorldPointCanvas>();
		w.position = tileWorldPosition;
	}

	public void ShowRepairEffect(GameTile tile) {
		ShowTileEffect(repairEffect, tile.worldPosition);
	}

	public void ShowDecayEffect(GameTile tile) {
		ShowTileEffect(decayEffect, tile.worldPosition);
	}

	void ShowInfoBubble(GameTile tile) {
		if (currentInfoBubble) {
			GameObject.Destroy(currentInfoBubble);
		}

		TileInfo tileInfo = Instantiate(infoBubbleTemplate, doubleScaleCanvas.transform);
		currentInfoBubble = tileInfo.gameObject;
		tileInfo.Initialize(tile);
	}

	public void HideInfoBubble() {
		GameObject.Destroy(currentInfoBubble);
		clickedTilemap.ClearAllTiles();
		highlightTilemap.ClearAllTiles();
		iconTilemap.ClearAllTiles();
		currentSelectedGridPosition = new Vector3Int(999, 999, 999);
	}

	void ShowTileIcon(TileHighlight highlight) {
		foreach (Vector3Int gridPos in highlight.targets) {
			iconTilemap.SetTile(gridPos, highlight.tile);
		}
	}

	public void ShowSingleIcon(Tile iconTile, Vector3Int gridPos) {
		singleIconTilemap.ClearAllTiles();
		singleIconTilemap.SetTile(gridMousePos, iconTile);
	}

	public void DisplayTileVisuals(GameTile tile) {
		clickedTilemap.ClearAllTiles();
		iconTilemap.ClearAllTiles();
		HighlightTile(tile.gridPosition);
		clickedTilemap.SetTile(tile.gridPosition, clickedTile);
		foreach (ITileHighlighter h in tile.GetComponents<ITileHighlighter>()) {
			ShowTileIcon(h.GetHighlight());
		}
	}

	public void OnGameBoardChanged() {
		// update the tile bubble in-place
		if (currentInfoBubble != null) {
			OnTileClick(currentSelectedGridPosition, silent:true);
		}
	}
}
